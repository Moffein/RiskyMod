using System;
using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.Legendary
{
    public class Aegis
    {
        public static bool enabled = true;

        public Aegis()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += AegisBarrierArmor;

            //Stacks no longer give extra barrier. Initial stack barrier increased.
            IL.RoR2.HealthComponent.Heal += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "barrierOnOverHeal")
                    ))
                {
                    if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "barrierOnOverHeal")
                    ))
                    {
                        c.EmitDelegate<Func<int, int>>(itemCount => 1);

                        if (c.TryGotoNext(
                         x => x.MatchLdcR4(0.5f)
                        ))
                        {
                            c.Next.Operand = 0.7f;
                            error = false;
                        }
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Aegis IL Hook failed");
                }
            };
        }

        private static void AegisBarrierArmor(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (sender.healthComponent && sender.healthComponent.barrier > 0f)
            {
                int itemCount = inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                args.armorAdd += 40f * itemCount;
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.BarrierOnOverHeal);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BarrierOnOverHeal);
        }
    }
}
