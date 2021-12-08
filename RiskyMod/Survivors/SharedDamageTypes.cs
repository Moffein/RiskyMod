using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors
{
    public class SharedDamageTypes
    {
        public static DamageAPI.ModdedDamageType ProjectileRainForce;
        public SharedDamageTypes()
        {
            ProjectileRainForce = DamageAPI.ReserveDamageType();

            TakeDamage.ModifyInitialDamageActions += ApplyProjectileRainForce;
        }


        private static void ApplyProjectileRainForce(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.ProjectileRainForce))
            {
                if (damageInfo.inflictor && damageInfo.inflictor.transform)
                {
                    Vector3 direction = -damageInfo.inflictor.transform.up;
                    CharacterBody cb = self.body;
                    if (cb)
                    {
                        //Scale force to match mass
                        Rigidbody rb = cb.rigidbody;
                        if (rb)
                        {
                            direction *= Mathf.Max(rb.mass / 100f, 1f);
                        }
                    }
                    damageInfo.force = 330f * direction;
                }
            }
        }
    }
}
