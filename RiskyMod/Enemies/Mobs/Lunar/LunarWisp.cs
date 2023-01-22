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
        public static bool enableFalloff = true;
        public static bool removeHitscan = true;
        public static bool reduceSpawnrate = true;

        public static GameObject shardProjectilePrefab;

        public LunarWisp()
        {
            if (enabled)
            {
                EnableStatusConditions();
                if (removeHitscan)
                {
                    RemoveHitscan();
                }
                else
                {
                    EnableBulletFalloff();
                }
                if (reduceSpawnrate)
                {
                    ReduceSpawnrate();
                }
            }
            ModifyProjectile();
        }

        private void ReduceSpawnrate()
        {
            CharacterSpawnCard csc = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/LunarWisp/cscLunarWisp.asset").WaitForCompletion();
            csc.directorCreditCost = 700; //550 vanilla, 350 for lunar golem
        }

        private void EnableStatusConditions()
        {
            GameObject enemyObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/lunarwispbody");
            SetStateOnHurt ssoh = enemyObject.GetComponent<SetStateOnHurt>();
            if (!ssoh)
            {
                ssoh = enemyObject.AddComponent<SetStateOnHurt>();
            }
            ssoh.hitThreshold = 0.5f;
            ssoh.canBeHitStunned = true;
            ssoh.canBeStunned = true;
            ssoh.canBeFrozen = true;

            EntityStateMachine body = null;
            EntityStateMachine weapon = null;
            EntityStateMachine[] stateMachines = enemyObject.GetComponents<EntityStateMachine>();
            foreach(EntityStateMachine esm in stateMachines)
            {
                switch(esm.customName)
                {
                    case "Body":
                        body = esm;
                        break;
                    case "Weapon":
                        weapon = esm;
                        break;
                    default:
                        break;
                }
            }

            ssoh.targetStateMachine = body;
            ssoh.idleStateMachine = new EntityStateMachine[] { weapon };
            ssoh.hurtState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.HurtStateFlyer));
        }

        private void ModifyProjectile()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/lunarwisptrackingbomb");

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

        private void EnableBulletFalloff()
        {
            IL.EntityStates.LunarWisp.FireLunarGuns.OnFireAuthority += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                    ))
                {
                    c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                    {
                        bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                        return bulletAttack;
                    });

                    if (c.TryGotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                    ))
                    {
                        c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                        {
                            bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                            return bulletAttack;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: LunarWisp IL Hook failed");
                }
            };
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
                ProjectileManager.instance.FireProjectile(shardProjectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), self.gameObject, damage, 0f, isCrit, DamageColorIndex.Default, null, 120f);
            };
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.LunarWisp.FireLunarGuns", "baseFireInterval", "0.125");
        }
    }
}
