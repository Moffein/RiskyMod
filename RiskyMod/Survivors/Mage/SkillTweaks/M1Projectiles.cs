using RoR2;
using RoR2.Projectile;
using R2API;
using UnityEngine;

namespace RiskyMod.Survivors.Mage
{
    public class M1Projectiles
    {
        public static GameObject FireBolt;
        public static GameObject LightningBolt;

        public static bool increaseRange = true;
        public static bool scaleBurnDamage = false;
        public static bool modifyPlasma = false;

        public M1Projectiles()
        {
            if (!increaseRange && !scaleBurnDamage && !modifyPlasma) return;

            if (increaseRange || scaleBurnDamage)
            {
                FireBolt = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/magefireboltbasic").InstantiateClone("RiskyMod_FlameBolt");
                IncreaseProjectileLifetime(FireBolt);
                if (scaleBurnDamage)
                {
                    ProjectileDamage pd = FireBolt.GetComponent<ProjectileDamage>();
                    pd.damageType = DamageType.Generic;
                    DamageAPI.ModdedDamageTypeHolderComponent mdc = FireBolt.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                    mdc.Add(SharedDamageTypes.IgniteLevelScaled);
                }
                ProjectileAPI.Add(FireBolt);
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireFireBolt", "projectilePrefab", FireBolt);
            }

            if (increaseRange || modifyPlasma)
            {
                LightningBolt = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/magelightningboltbasic").InstantiateClone("RiskyMod_PlasmaBolt");
                IncreaseProjectileLifetime(LightningBolt);
                if (modifyPlasma)
                {
                    ProjectileImpactExplosion pie = LightningBolt.GetComponent<ProjectileImpactExplosion>();
                    //pie.blastRadius = 2.5f;

                    ProjectileProximityBeamController pbc = LightningBolt.GetComponent<ProjectileProximityBeamController>();
                    if (!pbc)
                    {
                        pbc = LightningBolt.AddComponent<ProjectileProximityBeamController>();
                    }
                    pbc.attackFireCount = 3;
                    pbc.attackInterval = 0.06f;
                    pbc.attackRange = 7f;
                    pbc.listClearInterval = 10f;
                    pbc.minAngleFilter = 0f;
                    pbc.maxAngleFilter = 180f;
                    pbc.procCoefficient = 0.5f;
                    pbc.damageCoefficient = 0.8f / 2.2f;
                    pbc.bounces = 0;
                    pbc.lightningType = RoR2.Orbs.LightningOrb.LightningType.Ukulele;
                }
                ProjectileAPI.Add(LightningBolt);
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireLightningBolt", "projectilePrefab", LightningBolt);
            }
        }

        private void IncreaseProjectileLifetime(GameObject projectile)
        {
            if (!increaseRange) return;
            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;
        }
    }
}
