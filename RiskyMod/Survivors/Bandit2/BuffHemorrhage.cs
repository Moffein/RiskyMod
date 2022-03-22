using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BuffHemorrhage
    {
        public static bool enabled = true;
        public BuffHemorrhage()
        {
            if (!enabled) return;
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                if (damageInfo.dotIndex == RoR2.DotController.DotIndex.SuperBleed)
                {
                    if (self.body.armor > 0f)
                    {
                        damageInfo.damage *= (100f + self.body.armor + self.adaptiveArmorValue)/100f;
                    }
                }
                orig(self, damageInfo);
            };
        }
    }
}
