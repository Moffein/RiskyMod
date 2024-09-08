using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class LaserScope
    {
        public static bool enabled = true;
        
        public LaserScope()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += AddCrit;
        }

        private void AddCrit(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (inventory.GetItemCount(DLC1Content.Items.CritDamage) > 0) args.critAdd += 5f;
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.CritDamage);
        }
    }
}
