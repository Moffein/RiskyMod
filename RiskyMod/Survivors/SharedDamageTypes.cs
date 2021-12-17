using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors
{
    public class SharedDamageTypes
    {
        public static DamageAPI.ModdedDamageType ProjectileRainForce;
        public static DamageAPI.ModdedDamageType DebuffRegen;
        public static DamageAPI.ModdedDamageType InterruptOnHit;
        public static DamageAPI.ModdedDamageType Blight7s;

        public SharedDamageTypes()
        {
            ProjectileRainForce = DamageAPI.ReserveDamageType();
            DebuffRegen = DamageAPI.ReserveDamageType();
            InterruptOnHit = DamageAPI.ReserveDamageType();
            Blight7s = DamageAPI.ReserveDamageType();

            TakeDamage.ModifyInitialDamageActions += ApplyProjectileRainForce;
            OnHitEnemy.OnHitAttackerActions += ApplyDebuffRegen;
            OnHitEnemy.OnHitNoAttackerActions += ApplyInterruptOnHit;
            OnHitEnemy.OnHitNoAttackerActions += ApplyBlight7s;
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

        private static void ApplyDebuffRegen(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.DebuffRegen))
            {
                int debuffCount = 0;
                foreach (BuffIndex buffType in BuffCatalog.debuffBuffIndices)
                {
                    if (victimBody.HasBuff(buffType))
                    {
                        debuffCount++;
                    }
                }
                DotController dotController = DotController.FindDotController(victimBody.gameObject);
                if (dotController)
                {
                    for (DotController.DotIndex dotIndex = DotController.DotIndex.Bleed; dotIndex < DotController.DotIndex.Count; dotIndex++)
                    {
                        if (dotController.HasDotActive(dotIndex))
                        {
                            debuffCount++;
                        }
                    }
                }
                if (debuffCount > 0)
                {
                    attackerBody.AddTimedBuff(RoR2Content.Buffs.CrocoRegen, 0.025f * debuffCount * 10f);
                }
            }
        }

        private static void ApplyInterruptOnHit(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.InterruptOnHit))
            {
                SetStateOnHurt component = victimBody.healthComponent.GetComponent<SetStateOnHurt>();
                if (component != null)
                {
                    component.SetStun(-1f);
                }
            }
        }

        private static void ApplyBlight7s(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.Blight7s))
            {
                DotController.InflictDot(victimBody.gameObject, damageInfo.attacker, DotController.DotIndex.Blight, 7f, 1f);
            }
        }
    }
}
