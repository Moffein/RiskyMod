using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System;

namespace RiskyMod.Items.DLC3.Food
{
    public class SearedSteak
    {
        public static bool enabled = true;

        public SearedSteak()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC3Content.Items), "CookedSteak")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: SearedSteak IL Hook failed");
                }
            };
            GetStatCoefficients.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int steakCount = sender.inventory.GetItemCountEffective(DLC3Content.Items.CookedSteak);
            if (steakCount > 0)
            {
                args.baseHealthAdd += sender.levelMaxHealth * steakCount * 1.5f;
                args.healthMultAdd += 0.05f * steakCount;
            }
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC3Content.Items.CookedSteak);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC3Content.Items.CookedSteak);
        }
    }
}
