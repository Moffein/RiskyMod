using RoR2;
using RoR2.Projectile;
using R2API;
using UnityEngine;
using RiskyMod.MonoBehaviours;

namespace RiskyMod.Survivors.Mage
{
    public class M1Projectiles
    {
        public static GameObject FireBolt;
        public static GameObject LightningBolt;

        public static bool increaseRange = true;

        public M1Projectiles()
        {
            CreateFireBolt();
            CreateLightningBolt();
        }

        private static void CreateFireBolt()
        {
            if (FireBolt) return;
            FireBolt = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/magefireboltbasic").InstantiateClone("RiskyMod_FlameBolt");
            IncreaseProjectileLifetime(FireBolt);
            Content.Content.projectilePrefabs.Add(FireBolt);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireFireBolt", "projectilePrefab", FireBolt);
        }

        private static void CreateLightningBolt()
        {
            if (LightningBolt) return;
            LightningBolt = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/magelightningboltbasic").InstantiateClone("RiskyMod_PlasmaBolt");
            IncreaseProjectileLifetime(LightningBolt);
            Content.Content.projectilePrefabs.Add(LightningBolt);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireLightningBolt", "projectilePrefab", LightningBolt);
        }

        public static void ModifyFireBolt()
        {
            if (!FireBolt) CreateFireBolt();

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireFireBolt", "damageCoefficient", "3.9");
        }

        public static void ModifyLightningBolt()
        {
            if (!LightningBolt) CreateLightningBolt();

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireLightningBolt", "damageCoefficient", "3.9");
            /*ProjectileZapChainOnExplosion pzc = LightningBolt.AddComponent<ProjectileZapChainOnExplosion>();
            pzc.damageCoefficient = 0.9f / 2.7f;
            pzc.maxBounces = 20;
            pzc.initialTargets = 2;
            pzc.targetsPerBounce = 2;
            pzc.procCoefficient = 0.5f;
            pzc.range = 10f;
            pzc.requireHit = true;*/

            /*ProjectileImpactExplosion pie = LightningBolt.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = 2.5f; //Same as Fire Bolt now*/
        }

        private static void IncreaseProjectileLifetime(GameObject projectile)
        {
            if (!increaseRange) return;
            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;
        }
    }
}
