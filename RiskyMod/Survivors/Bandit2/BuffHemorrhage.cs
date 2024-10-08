﻿using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BuffHemorrhage
    {
        public static bool enabled = true;

        public static bool enableProcs = true;
        public static bool enableCrit = false;
        public BuffHemorrhage()
        {
            if (!enabled || (!enableProcs && !enableCrit)) return;
            On.RoR2.HealthComponent.TakeDamageProcess += (orig, self, damageInfo) =>
            {
                bool procHemmorrhage = false;
                if (damageInfo.dotIndex == RoR2.DotController.DotIndex.SuperBleed && damageInfo.damageType.damageType.HasFlag(DamageType.DoT) && !damageInfo.damageType.damageType.HasFlag(DamageType.AOE) && damageInfo.procCoefficient == 0f)
                {
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
                }
            };
        }
    }
}
