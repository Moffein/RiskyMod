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
            SharedHooks.TakeDamage.ModifyInitialDamageNoAttackerActions += NullifyVengeanceFallDamage;
        }

        private void NullifyVengeanceFallDamage(DamageInfo damageInfo, HealthComponent self)
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
