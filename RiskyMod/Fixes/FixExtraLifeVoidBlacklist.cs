using RoR2;

namespace RiskyMod.Fixes
{
    public class FixExtraLifeVoidBlacklist
    {
        public static bool enabled = true;
        public FixExtraLifeVoidBlacklist()
        {
            if (!enabled) return;
            On.RoR2.Items.ContagiousItemManager.TryForceReplacement += (orig, inventory, replacementItemIndex) =>
            {
                bool isValidItem = Run.instance.availableItems.Contains(replacementItemIndex);
                if (!isValidItem) replacementItemIndex = ItemIndex.None;
                orig(inventory, replacementItemIndex);
            };
        }
    }
}
