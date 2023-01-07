using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BuffHemorrhage
    {
        public static bool enabled = true;

        public static bool ignoreArmor = true;
        public static bool enableProcs = true;
        public static bool enableCrit = false;
        public BuffHemorrhage()
        {
            if (!enabled || (!ignoreArmor && !enableProcs && !enableCrit)) return;
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool procHemmorrhage = false;
                if (damageInfo.dotIndex == RoR2.DotController.DotIndex.SuperBleed && damageInfo.damageType.HasFlag(DamageType.DoT) && damageInfo.procCoefficient == 0f)
                {
                    if (ignoreArmor)
                    {
                        float totalArmor = self.body.armor + self.adaptiveArmorValue;
                        if (totalArmor > 0f)
                        {
                            damageInfo.damage *= (100f + totalArmor) / 100f;
                        }
                    }

                    if (enableProcs)
                    {
                        damageInfo.procCoefficient = 0.5f;
                        procHemmorrhage = true;
                    }

                    if (enableCrit)
                    {
                        float baseCritMult = BackstabRework.enabled ? 1.5f : 2f;
                        damageInfo.damage /= baseCritMult;
                        damageInfo.crit = true;
                    }
                }
                orig(self, damageInfo);

                if (procHemmorrhage)
                {
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, self.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, self.gameObject);
                }
            };
        }
    }
}
