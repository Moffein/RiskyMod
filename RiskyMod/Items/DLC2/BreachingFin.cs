using RoR2;

namespace RiskyMod.Items.DLC2
{
    public class BreachingFin
    {
        public static bool enabled = true;

        public BreachingFin()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            RiskyFixes.Fixes.Items.FinReproc.damageBuffPerStack = 0.15f;
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC2Content.Items.KnockBackHitEnemies);
        }
    }
}
