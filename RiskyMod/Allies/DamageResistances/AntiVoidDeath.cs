using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DamageResistances
{
    public class AntiVoidDeath
    {

        public static bool enabled = true;

        public AntiVoidDeath()
        {
            if (!enabled) return;

            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            if (NetworkServer.active && AlliesCore.IsDrone(self))
            {
                if ((damageInfo.damageType & DamageType.VoidDeath) > 0 && (damageInfo.damageType & DamageType.BypassOneShotProtection) == 0)
                {
                    damageInfo.damageType.damageType &= ~DamageType.VoidDeath;
                    //damageInfo.damageType.damageType |= DamageType.NonLethal;

                    //Overwrite damage since actual damage numbers of void implosions are low
                    float totalDamage = self.fullCombinedHealth * 0.5f;
                    float totalArmor = self.adaptiveArmorValue + self.body.armor;
                    if (totalArmor >= 0f)
                    {
                        totalDamage /= 100f / (100f + totalArmor);
                    }
                    else
                    {
                        totalDamage /= 2 - 100 / (100 - totalArmor);
                    }

                    damageInfo.damage = totalDamage;
                }
            }
            orig(self, damageInfo);
        }
    }
}
