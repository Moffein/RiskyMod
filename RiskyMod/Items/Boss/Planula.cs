namespace Risky_Mod.Items.Boss
{
    //This is just here to prevent anti-synergy with the Stealthkit/Razorwire/Polyp changes (Planula healing before HP lost is calculated)
    public class Planula
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove vanilla effect
            On.RoR2.HealthComponent.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                self.itemCounts.parentEgg = 0;
            };

            //Effect handled in SharedHooks.TakeDamage
        }
    }
}
