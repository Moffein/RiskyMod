using R2API;

namespace Risky_ItemTweaks.Items.Uncommon
{
    public class Razorwire
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove vanilla effect
            On.RoR2.HealthComponent.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                self.itemCounts.thorns = 0;
            };

            LanguageAPI.Add("ITEM_THORNS_DESC", "Getting hit causes you to explode in a burst of razors, dealing <style=cIsDamage>80%-420% damage</style>. Hits up to <style=cIsDamage>5</style> <style=cStack>(+2 per stack)</style> targets in a <style=cIsDamage>16m-40m</style> radius. Damage and radius increases the more damage taken.");

            //Effect handled in SharedHooks.TakeDamage
        }
    }
}
