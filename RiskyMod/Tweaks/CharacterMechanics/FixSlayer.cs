using RiskyMod.SharedHooks;
using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class FixSlayer
    {
        public static bool enabled = true;
        public FixSlayer()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageNoAttackerActions += SlayerDamage;
        }

        private static void SlayerDamage(DamageInfo damageInfo, HealthComponent self)
        {

            if ((damageInfo.damageType & DamageType.BonusToLowHealth) == DamageType.BonusToLowHealth)
            {
                damageInfo.damageType &= ~DamageType.BonusToLowHealth;
                damageInfo.damage *= Mathf.Lerp(3f, 1f, self.combinedHealthFraction);
            }
        }
    }
}
