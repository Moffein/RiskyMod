using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.Survivors.Treebot;
using RoR2;
using System;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class Scorpion
    {
        public static bool enabled = true;

        public Scorpion()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "PermanentDebuffOnHit")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);    //HealthComponent
                    c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                    c.EmitDelegate<Func<BuffDef, HealthComponent, DamageInfo, BuffDef>> ((buff, self, damageInfo) =>
                    {
                        return (self.gameObject != damageInfo.attacker ? buff : null);
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Scorpion IL Hook failed");
                }
            };
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int scorpionCount = sender.GetBuffCount(DLC1Content.Buffs.PermanentDebuff);
            args.armorAdd += scorpionCount; //Vanilla is -= 2 * scorpionCount
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.PermanentDebuffOnHit);
        }
    }
}
