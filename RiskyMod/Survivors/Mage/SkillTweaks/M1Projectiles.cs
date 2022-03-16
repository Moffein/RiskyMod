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

        public M1Projectiles()
        {
            if (!increaseRange) return;

            if (increaseRange)
            {
                FireBolt = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/magefireboltbasic").InstantiateClone("RiskyMod_FlameBolt");
                IncreaseProjectileLifetime(FireBolt);
                Content.Content.projectilePrefabs.Add(FireBolt);
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireFireBolt", "projectilePrefab", FireBolt);
            }

            if (increaseRange)
            {
                LightningBolt = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/magelightningboltbasic").InstantiateClone("RiskyMod_PlasmaBolt");
                IncreaseProjectileLifetime(LightningBolt);
                Content.Content.projectilePrefabs.Add(LightningBolt);
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
