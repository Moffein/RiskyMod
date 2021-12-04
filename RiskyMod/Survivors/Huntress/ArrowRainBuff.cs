using RoR2;
using UnityEngine;
using RoR2.Projectile;
using R2API;
using RiskyMod.SharedHooks;

namespace RiskyMod.Survivors.Huntress
{
    public class ArrowRainBuff
    {
        public static bool enabled = true;
        public static GameObject arrowRainObject;
        public static GameObject arrowRainScepterObject;
        public static DamageAPI.ModdedDamageType ArrowRainForce;
        public static DamageAPI.ModdedDamageType ArrowRainScepterForce;

        public ArrowRainBuff()
        {
            if (!enabled) return;

            ArrowRainForce = DamageAPI.ReserveDamageType();

            arrowRainObject = Resources.Load<GameObject>("prefabs/projectiles/huntressarrowrain").InstantiateClone("RiskyModArrowRainProjectile", true);
            ProjectileAPI.Add(arrowRainObject);

            DamageAPI.ModdedDamageTypeHolderComponent mdh = arrowRainObject.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdh.Add(ArrowRainForce);

            ProjectileDotZone arrowDotZone = arrowRainObject.GetComponent<ProjectileDotZone>();
            arrowDotZone.overlapProcCoefficient = 0.7f;
            arrowDotZone.damageCoefficient = 1f / 3f; //Adjusts DPS value to match the entitystate damagecoefficient
            //arrowDotZone.forceVector = 

            HitBox hb = arrowRainObject.GetComponentInChildren<HitBox>();
            hb.transform.localPosition += 0.5f * Vector3.up;
            hb.transform.localScale = new Vector3(hb.transform.localScale.x, 2.5f * hb.transform.localScale.y, hb.transform.localScale.z);

            TakeDamage.ModifyInitialDamageActions += ApplyArrowRainForce;
        }

        private static void ApplyArrowRainForce(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(ArrowRainForce))
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
