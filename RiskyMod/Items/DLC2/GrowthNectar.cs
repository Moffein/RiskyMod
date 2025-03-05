using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC2
{
    public class GrowthNectar
    {
        public static bool enabled = true;

        public GrowthNectar()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "BoostAllStatsBuff"))
                && c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.04f)))
            {
                c.EmitDelegate<Func<float, float>>(x => 0.05f);
            }
            else
            {
                Debug.LogError("RiskyMod: Growth Nectar IL Hook failed.");
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC2Content.Items.BoostAllStats);
        }
    }
}
