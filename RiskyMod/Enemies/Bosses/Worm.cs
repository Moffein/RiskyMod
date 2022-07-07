using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace RiskyMod.Enemies.Bosses
{
    public class Worm
    {
        public static bool enabled = true;

        public static BodyIndex MagmaWormIndex;
        public static BodyIndex ElectricWormIndex;

        public static GameObject MagmaWormProjectile;
        public static GameObject ElectricWormProjectile;
        public static GameObject MagmaWormProjectileZone;
        public static GameObject ElectricWormProjectileZone;

        public Worm()
        {
            if (!enabled) return;

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                MagmaWormIndex = BodyCatalog.FindBodyIndex("MagmaWormBody");
                ElectricWormIndex = BodyCatalog.FindBodyIndex("ElectricWormBody");
            };

            //ProjectileSetup();
            AlwaysFireMeatballs();
            PrioritizePlayers();
            ReduceFollowDelay(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion());
            ReduceFollowDelay(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElectricWorm/ElectricWormBody.prefab").WaitForCompletion());
        }

        //This causes the ground to constantly be filled with fire. Might not be suitable.
        private void ProjectileSetup()
        {
            MagmaWormProjectileZone = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Molotov/MolotovProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("RiskyMod_MagmaWormProjectileZone", true);
            Content.Content.projectilePrefabs.Add(MagmaWormProjectileZone);
            ProjectileDamage pd = MagmaWormProjectileZone.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            MagmaWormProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaOrbProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyMod_MagmaWormProjectile", true);
            Content.Content.projectilePrefabs.Add(MagmaWormProjectile);
            ProjectileImpactExplosion pie = MagmaWormProjectile.GetComponent<ProjectileImpactExplosion>();
            pie.fireChildren = true;
            pie.childrenProjectilePrefab = MagmaWormProjectileZone;
            pie.childrenDamageCoefficient = 1f;

            GameObject magmaWormPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
            WormBodyPositions2 magmaWormController = magmaWormPrefab.GetComponent<WormBodyPositions2>();
            magmaWormController.meatballProjectile = MagmaWormProjectile;
        }

        private void AlwaysFireMeatballs()
        {
            On.EntityStates.MagmaWorm.SwitchStance.SetStanceParameters += (orig, self, leaping) =>
            {
                orig(self, leaping);
                if (!leaping)
                {
                    WormBodyPositions2 component = self.GetComponent<WormBodyPositions2>();
                    if (component)
                    {
                        component.shouldFireMeatballsOnImpact = true;
                    }
                }
            };
        }

        private void PrioritizePlayers()
        {
            On.RoR2.CharacterAI.BaseAI.UpdateTargets += (orig, self) =>
            {
                orig(self);
                if (self.body && (self.body.bodyIndex == MagmaWormIndex || self.body.bodyIndex == ElectricWormIndex))
                {
                    if (self.currentEnemy != null
                    && self.currentEnemy.characterBody
                    && !self.currentEnemy.characterBody.isPlayerControlled
                    && self.currentEnemy.characterBody.bodyIndex != Items.Uncommon.SquidPolyp.squidTurretBodyIndex
                    && self.currentEnemy.characterBody.teamComponent)
                    {
                        TeamIndex enemyTeam = self.currentEnemy.characterBody.teamComponent.teamIndex;

                        List<CharacterBody> targetList = new List<CharacterBody>();
                        foreach (PlayerCharacterMasterController pc in PlayerCharacterMasterController.instances)
                        {
                            if (pc.body && pc.body.teamComponent && pc.body.teamComponent.teamIndex == enemyTeam && pc.body.isPlayerControlled && pc.body.healthComponent && pc.body.healthComponent.alive)
                            {
                                targetList.Add(pc.body);
                            }
                        }

                        if (targetList.Count > 0)
                        {
                            Vector3 myPos = self.body.corePosition;
                            float shortestDistSqr = Mathf.Infinity;
                            CharacterBody newTarget = null;

                            foreach (CharacterBody cb in targetList)
                            {
                                float sqrDist = (myPos - cb.corePosition).sqrMagnitude;
                                if (sqrDist < shortestDistSqr)
                                {
                                    shortestDistSqr = sqrDist;
                                    newTarget = cb;
                                }
                            }

                            if (newTarget)
                            {
                                //Copied from Squid Polyp code
                                //Debug.Log("Changing worm target");
                                self.currentEnemy.gameObject = newTarget.gameObject;
                                self.currentEnemy.bestHurtBox = newTarget.mainHurtBox;
                                self.enemyAttention = self.enemyAttentionDuration;
                                self.targetRefreshTimer = 10f;
                                self.BeginSkillDriver(self.EvaluateSkillDrivers());
                            }
                        }
                    }
                }
            };
        }

        private void ReduceFollowDelay(GameObject bodyObject)
        {
            WormBodyPositions2 wbp = bodyObject.GetComponent<WormBodyPositions2>();
            if (wbp)
            {
                //Debug.Log("Follow Delay: " + wbp.followDelay);
                wbp.followDelay = 0.1f;
            }
        }
    }
}
