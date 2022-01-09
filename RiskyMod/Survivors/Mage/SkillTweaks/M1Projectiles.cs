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
        public static bool scaleBurnDamage = true;
        public static bool modifyPlasma = true;

        public M1Projectiles()
        {
            if (!increaseRange && !scaleBurnDamage && !modifyPlasma) return;

            if (increaseRange || scaleBurnDamage)
            {
                FireBolt = Resources.Load<GameObject>("prefabs/projectiles/magefireboltbasic").InstantiateClone("RiskyMod_FlameBolt");
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

                LightningBolt = Resources.Load<GameObject>("prefabs/projectiles/magelightningboltbasic").InstantiateClone("RiskyMod_PlasmaBolt");
                IncreaseProjectileLifetime(LightningBolt);
                if (modifyPlasma)
                {
                    ProjectileImpactExplosion pie = LightningBolt.GetComponent<ProjectileImpactExplosion>();
                    pie.blastRadius = 2.5f;
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
