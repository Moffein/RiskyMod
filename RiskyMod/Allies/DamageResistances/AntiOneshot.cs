using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DamageResistances
{
    public class AntiOneshot
    {
        public static bool enabled = true;

        public AntiOneshot()
        {
            if (!enabled) return;

            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            if (NetworkServer.active && AlliesCore.IsDrone(self))
            {
                float totalDamage = damageInfo.damage;
                float totalArmor = self.adaptiveArmorValue + self.body.armor;
                if (totalArmor >= 0f)
                {
                    totalDamage *= 100f / (100f + totalArmor);
                }
                else
                {
                    totalDamage *= 2 - 100 / (100 - totalArmor);
                }

                float maxDamage = self.fullCombinedHealth * 0.6f;
                if (totalDamage > maxDamage)
                {
                    damageInfo.damage *= maxDamage / totalDamage;
                }
            }
            orig(self, damageInfo);
        }
    }
}
