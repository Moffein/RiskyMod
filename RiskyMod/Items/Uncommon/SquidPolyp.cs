using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Uncommon
{
    public class SquidPolyp
    {
        public static bool enabled = true;
        public static GameObject procEffectPrefab;
        public static bool scaleCount = false;

        public static BodyIndex SquidTurretBodyIndex;

        //Does this need turretblacklist?
        public SquidPolyp()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Squid);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Squid);

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnInteractionBegin += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Squid")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            procEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/claygooorbimpact").InstantiateClone("RiskyItemTweaks_SquidPolypProc", false);
            EffectComponent ec = procEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_treeBot_m2_launch";
            EffectAPI.AddEffect(procEffectPrefab);

            TakeDamage.HandleOnPercentHpLostActions += OnHpLost;
            TakeDamage.OnDamageTakenAttackerActions += DistractOnHit;

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                SquidPolyp.SquidTurretBodyIndex = BodyCatalog.FindBodyIndex("SquidTurretBody");
            };
        }

        private static void DistractOnHit(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            //Squid Turrets are guaranteed to draw aggro upon dealing damage.
            //Based on https://github.com/DestroyedClone/PoseHelper/blob/master/HighPriorityAggroTest/HPATPlugin.cs
            if (attackerBody.bodyIndex == SquidPolyp.SquidTurretBodyIndex)
            {
                if (self.body.master && self.body.master.aiComponents.Length > 0)
                {
                    foreach (BaseAI ai in self.body.master.aiComponents)
                    {
                        ai.currentEnemy.gameObject = attackerBody.gameObject;
                        ai.currentEnemy.bestHurtBox = attackerBody.mainHurtBox;
                        ai.enemyAttention = ai.enemyAttentionDuration;
                        ai.targetRefreshTimer = 5f;
                        ai.BeginSkillDriver(ai.EvaluateSkillDrivers());
                    }
                }
            }
        }

        private static void OnHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost)
        {
            int polypCount = inventory.GetItemCount(RoR2Content.Items.Squid);
            if (polypCount > 0)
            {
                if (percentHpLost > 0f)
                {
                    if (Util.CheckRoll(percentHpLost, self.body.master))
                    {
                        SquidMinionComponent sq = self.gameObject.GetComponent<SquidMinionComponent>();
                        if (!sq)
                        {
                            sq = self.gameObject.AddComponent<SquidMinionComponent>();
                        }
                        if (sq.CanSpawnSquid())
                        {
                            EffectManager.SimpleEffect(SquidPolyp.procEffectPrefab, self.body.corePosition, Quaternion.identity, true);
                            SpawnCard spawnCard = Resources.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscSquidTurret");
                            DirectorPlacementRule placementRule = new DirectorPlacementRule
                            {
                                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                                minDistance = 5f,
                                maxDistance = 25f,
                                position = self.body.corePosition
                            };
                            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng);
                            directorSpawnRequest.teamIndexOverride = self.body.teamComponent.teamIndex;
                            directorSpawnRequest.summonerBodyObject = self.gameObject;
                            directorSpawnRequest.ignoreTeamMemberLimit = true;  //Polyps should always be able to spawn. Does this need a cap for performance?
                            directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
                            {
                                if (!result.success)
                                {
                                    return;
                                }
                                CharacterMaster component6 = result.spawnedInstance.GetComponent<CharacterMaster>();
                                component6.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                                if (self.itemCounts.invadingDoppelganger > 0)
                                {
                                    //Doppelganger Turrets decay faster and deal less damage.
                                    component6.inventory.GiveItem(RoR2Content.Items.InvadingDoppelganger);
                                    component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 15);
                                }
                                else
                                {
                                    component6.inventory.GiveItem(RoR2Content.Items.BoostHp, 3 * (polypCount - 1));
                                    component6.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, 10 * (polypCount - 1));
                                    component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 30);
                                }
                                sq.AddSquid(result.spawnedInstance);
                            }));
                            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                        }
                    }
                }
            }
        }
    }

    public class SquidMinionComponent : MonoBehaviour
    {
        private List<GameObject> squidList;
        private Inventory inventory = null;

        public void Awake()
        {
            squidList = new List<GameObject>();

            CharacterBody cb = base.gameObject.GetComponent<CharacterBody>();
            if (cb && cb.inventory)
            {
                inventory = cb.inventory;
            }
        }

        public bool CanSpawnSquid()
        {
            if (SquidPolyp.scaleCount)
            {
                return squidList.Count < 1 + (inventory ? inventory.GetItemCount(RoR2Content.Items.Squid) : 0);
            }
            else
            {
                return squidList.Count < 3;
            }
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                UpdateSquids();
            }
        }

        private void UpdateSquids()
        {
            List<GameObject> toRemove = new List<GameObject>();
            foreach (GameObject sm in squidList)
            {
                if (!sm.gameObject)
                {
                    toRemove.Add(sm);
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (GameObject sm in toRemove)
                {
                    squidList.Remove(sm);
                }
                toRemove.Clear();
            }
        }

        public void AddSquid(GameObject go)
        {
            squidList.Add(go);
        }
    }
}
