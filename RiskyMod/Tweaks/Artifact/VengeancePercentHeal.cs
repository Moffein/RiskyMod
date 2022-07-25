using RoR2;

namespace RiskyMod.Tweaks.Artifact
{
    public class VengeancePercentHeal
    {
        public static bool enabled = true;
        public VengeancePercentHeal()
        {
            if (!enabled) return;

            On.RoR2.HealthComponent.HealFraction += (orig, self, fraction, procChainMask) =>
            {
                if (self.itemCounts.invadingDoppelganger > 0)
                {
                    fraction *= 0.1f;
                }
                return orig(self, fraction, procChainMask);
            };
        }
    }
}
