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

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireFireBolt", "damageCoefficient", "3.6");   //Gets repeated 5x
            /*DamageAPI.ModdedDamageTypeHolderComponent mdc = FireBolt.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(SharedDamageTypes.RepeatHit);

            ProjectileController pc = FireBolt.GetComponent<ProjectileController>();
            pc.procCoefficient = 0.75f;*/
        }

        private static void IncreaseProjectileLifetime(GameObject projectile)
        {
            if (!increaseRange) return;
            ProjectileSimple ps = projectile.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;
        }
    }
}
