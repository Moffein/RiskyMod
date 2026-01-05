using R2API;
using RiskyMod.SharedHooks;
using RoR2;

namespace RiskyMod.Items.DLC3.Boss
{
    public class PrisonMatrix
    {
        public static bool enabled = true;

        public PrisonMatrix()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            GetStatCoefficients.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (sender.inventory.GetItemCountEffective(DLC3Content.Items.PowerCube) > 0)
            {
                args.armorAdd += 10;
            }
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC3Content.Items.PowerCube);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC3Content.Items.PowerCube);
        }
    }
}
