using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC2
{
    public class BolsteringLantern
    {
        public static bool enabled = true;
        
        public BolsteringLantern()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            IL.RoR2.AttackSpeedPerNearbyCollider.UpdateValues += AttackSpeedPerNearbyCollider_UpdateValues;
        }

        private void AttackSpeedPerNearbyCollider_UpdateValues(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcI4(2)))
            {
                c.EmitDelegate<Func<int, int>>(x =>
                {
                    return 1;
                });
            }
            else
            {
                Debug.LogError("RiskyMod: Bolstering Lantern Collider IL Hook failed.");
            }
        }

        private void CharacterBody_RecalculateStats(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "AttackSpeedPerNearbyAllyOrEnemyBuff"))
                && c.TryGotoNext(MoveType.After, x=>x.MatchLdcR4(0.075f)))
            {
                c.EmitDelegate<Func<float, float>>(x => 0.12f);
            }
            else
            {
                Debug.LogError("RiskyMod: Bolstering Lantern IL Hook failed.");
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC2Content.Items.AttackSpeedPerNearbyAllyOrEnemy);
        }
    }
}
