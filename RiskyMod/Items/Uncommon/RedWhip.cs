using System;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;

namespace RiskyMod.Items.Uncommon
{
    public class RedWhip
    {
        public static bool enabled = true;
        public RedWhip()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            GetStatCoefficients.HandleStatsInventoryActions += HandleStats;
        }

        private static void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int itemCount = inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat);
            if (itemCount > 0)
            {
                args.baseMoveSpeedAdd += sender.baseMoveSpeed * 0.1f * itemCount;
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.SprintOutOfCombat);
        }
    }
}
