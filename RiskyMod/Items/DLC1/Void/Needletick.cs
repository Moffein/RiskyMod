using RoR2;
using R2API;

namespace RiskyMod.Items.DLC1.Void
{
    public class Needletick
    {
        public static bool enabled = true;

        public Needletick()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                int collapseCount = sender.GetBuffCount(DLC1Content.Buffs.Fracture);
                args.armorAdd -= collapseCount;
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.BleedOnHitVoid);
        }
    }
}
