using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BuffHemorrhage
    {
        public static bool enabled = true;

        public static bool ignoreArmor = true;
        public static bool enableProcs = true;
        public BuffHemorrhage()
        {
            if (!enabled || (!ignoreArmor && !enableProcs)) return;
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                if (damageInfo.dotIndex == RoR2.DotController.DotIndex.SuperBleed)
                {
                    if (ignoreArmor)
                    {
                        float totalArmor = self.body.armor + self.adaptiveArmorValue;
                        if (totalArmor > 0f)
                        {
                            damageInfo.damage *= (100f + totalArmor) / 100f;
                        }
                    }

                    if (enableProcs) damageInfo.procCoefficient = 0.2f;
                }
                orig(self, damageInfo);
            };
        }
    }
}
