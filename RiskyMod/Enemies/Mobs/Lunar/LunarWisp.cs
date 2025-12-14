using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EntityStates.LunarWisp;
using R2API;

namespace RiskyMod.Enemies.Mobs.Lunar
{
    public class LunarWisp
    {
        public static bool enabled = true;
        public static bool disableProjectileOnKill = true;
        public static bool removeHitscan = true;

        public static GameObject shardProjectilePrefab;

        public LunarWisp()
        {
            if (enabled)
            {
                if (removeHitscan)
                {
                    RemoveHitscan();
                }
            }
            ModifyProjectile();
        }

        private void ModifyProjectile()
        {
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarWisp/LunarWispTrackingBomb.prefab").WaitForCompletion();

            if (disableProjectileOnKill)
            {
                HealthComponent hc = projectile.GetComponent<HealthComponent>();
                hc.globalDeathEventChanceCoefficient = 0f;
            }

            if (enabled)
            {
                ProjectileImpactExplosion pie = projectile.GetComponent<ProjectileImpactExplosion>();
                pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            }
        }

        private void RemoveHitscan()
        {
            shardProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/LunarShardProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModLunarWispShardProjectile", true);
            Content.Content.projectilePrefabs.Add(shardProjectilePrefab);

            ProjectileImpactExplosion pie = shardProjectilePrefab.GetComponent<ProjectileImpactExplosion>();
            if (pie) pie.blastDamageCoefficient = 1f;

            ProjectileDirectionalTargetFinder pdtf = shardProjectilePrefab.GetComponent<ProjectileDirectionalTargetFinder>();
            if (pdtf) pdtf.lookCone = 15f;  //was 60f
            //UnityEngine.Object.Destroy(shardProjectilePrefab.GetComponent<ProjectileDirectionalTargetFinder>());
            //UnityEngine.Object.Destroy(shardProjectilePrefab.GetComponent<ProjectileSteerTowardTarget>());
            //UnityEngine.Object.Destroy(shardProjectilePrefab.GetComponent<ProjectileTargetComponent>());

            //Completly override the vanilla version.
            On.EntityStates.LunarWisp.FireLunarGuns.OnFireAuthority += (orig, self) =>
            {
                self.UpdateCrits();
                bool isCrit = !self.critEndTime.hasPassed;
                float damage = FireLunarGuns.baseDamagePerSecondCoefficient / self.baseBulletsPerSecond * self.damageStat;   //Supposed to be firing 2 bulletattacks.
                self.StartAimMode(0.5f, false);
                Ray aimRay = self.GetAimRay();
                ProjectileManager.instance.FireProjectile(shardProjectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), self.gameObject, damage, 0f, isCrit, DamageColorIndex.Default, null, 150f);
            };
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/LunarWisp/EntityStates.LunarWisp.FireLunarGuns.asset", "baseFireInterval", "0.125");
        }
    }
}
