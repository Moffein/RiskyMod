using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.Artifact
{
    public class NoVengeanceFallDamage
    {
        public static bool enabled = true;

        public NoVengeanceFallDamage()
        {
            if (!enabled) return;
            SharedHooks.TakeDamage.ModifyInitialDamageActions += NullifyVengeanceFallDamage;
        }

        private void NullifyVengeanceFallDamage(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.damageType.HasFlag(DamageType.FallDamage))
            {
                if (self.itemCounts.invadingDoppelganger > 0)
                {
                    damageInfo.damage = 0f;
                    damageInfo.rejected = true;
                }
            }
        }
    }
}
