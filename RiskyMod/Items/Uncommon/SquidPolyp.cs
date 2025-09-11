using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Items.Uncommon
{
    public class SquidPolyp
    {
        public static bool enabled = true;
        public static GameObject procEffectPrefab;
        public static bool scaleCount = false;
        public static bool ignoreAllyCap = true;
        public static bool weakToMithrix = true;

        public static bool cannotCopy = false;
        public static int minionSquidLimit = -1;

        public static BodyIndex squidTurretBodyIndex;
        public static CharacterSpawnCard squidTurretCard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Squid/cscSquidTurret.asset").WaitForCompletion();

        //Does this need turretblacklist?
        public SquidPolyp()
        {
            RoR2Application.onLoad += OnLoad;
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnInteractionBegin += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Squid")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: SquidPolyp IL Hook failed");
                }
            };

            procEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/goolake/ClayGooOrbImpact.prefab").WaitForCompletion().InstantiateClone("RiskyMod_SquidPolypProc", false);
            EffectComponent ec = procEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_treeBot_m2_launch";
            Content.Content.effectDefs.Add(new EffectDef(procEffectPrefab));

            TakeDamage.OnPercentHpLostActions += OnHpLost;

            GameObject squidBodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Squid/SquidTurretBody.prefab").WaitForCompletion();
            CharacterBody cb = squidBodyObject.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 100f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseArmor = 20f;

            if (weakToMithrix)
            {
                SharedHooks.TakeDamage.ModifyInitialDamageAttackerActions += WeakToMithrixHook;
            }
        }

        private void OnLoad()
        {
            SquidPolyp.squidTurretBodyIndex = BodyCatalog.FindBodyIndex("SquidTurretBody");
            if (enabled && ItemsCore.enabled) TakeDamage.distractOnHitBodies.Add(SquidPolyp.squidTurretBodyIndex);
        }

        private void WeakToMithrixHook(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (self.body.bodyIndex == squidTurretBodyIndex
                && (attackerBody.bodyIndex == Enemies.Mithrix.MithrixCore.brotherBodyIndex
                || attackerBody.bodyIndex == Enemies.Mithrix.MithrixCore.brotherHurtBodyIndex))
            {
                damageInfo.damage *= 2f;
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Squid);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Squid);
            if (SoftDependencies.AIBlacklistUseVanillaBlacklist) SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.Squid, ItemTag.AIBlacklist);
            if (cannotCopy) SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.Squid, ItemTag.CannotCopy);
        }

        private static void OnHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost)
        {
            int polypCount = inventory.GetItemCount(RoR2Content.Items.Squid);
            if (polypCount > 0)
            {
                if (percentHpLost > 0f)
                {
                    float chance = Mathf.Max(5f, 100f * percentHpLost / 75f);
                    if (Util.CheckRoll(chance, self.body.master))
                    {
                        SquidMinionComponent sq = self.gameObject.GetComponent<SquidMinionComponent>();
                        if (!sq)
                        {
                            sq = self.gameObject.AddComponent<SquidMinionComponent>();
                        }
                        if (sq.CanSpawnSquid())
                        {
                            EffectManager.SimpleEffect(SquidPolyp.procEffectPrefab, self.body.corePosition, Quaternion.identity, true);
                            SpawnCard spawnCard = squidTurretCard;
                            DirectorPlacementRule placementRule = new DirectorPlacementRule
                            {
                                placementMode = DirectorPlacementRule.PlacementMode.Approximate,    //Set to approximate since NearestNode causes overlapping
                                minDistance = 5f,
                                maxDistance = 25f,
                                position = self.body.corePosition
                            };
                            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng);
                            directorSpawnRequest.teamIndexOverride = self.body.teamComponent.teamIndex;
                            directorSpawnRequest.summonerBodyObject = self.gameObject;
                            directorSpawnRequest.ignoreTeamMemberLimit = SquidPolyp.ignoreAllyCap;
                            directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
                            {
                                if (result.success)
                                {
                                    CharacterMaster component6 = result.spawnedInstance.GetComponent<CharacterMaster>();
                                    if (component6.inventory)
                                    {
                                        if (component6.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) component6.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                                        if (self.itemCounts.invadingDoppelganger > 0)
                                        {
                                            //Doppelganger Turrets decay faster and deal less damage.
                                            component6.inventory.GiveItem(RoR2Content.Items.InvadingDoppelganger);
                                            component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 10);
                                        }
                                        else
                                        {
                                            component6.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, 10 * (polypCount - 1));

                                            if (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player && !SoftDependencies.KingKombatArenaActive)
                                            {
                                                component6.inventory.GiveItem(Allies.AllyItems.AllyMarkerItem);
                                                component6.inventory.GiveItem(Allies.AllyItems.AllyScalingItem);
                                                component6.inventory.GiveItem(Allies.AllyItems.AllyAllowOverheatDeathItem);
                                                component6.inventory.GiveItem(Allies.AllyItems.AllyAllowVoidDeathItem);
                                                component6.inventory.GiveItem(RoR2Content.Items.BoostHp, 30 + 4 * 5 * (polypCount - 1));
                                                component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 40);
                                            }
                                            else
                                            {
                                                component6.inventory.GiveItem(RoR2Content.Items.BoostHp, 5 * (polypCount - 1));
                                                component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 20);
                                            }
                                        }
                                    }
                                    sq.AddSquid(result.spawnedInstance);
                                }
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
        private bool isMinion;
        private CharacterBody cb;

        public void Awake()
        {
            squidList = new List<GameObject>();
            isMinion = false;

            cb = base.gameObject.GetComponent<CharacterBody>();
            if (cb && cb.inventory)
            {
                inventory = cb.inventory;
            }
        }

        public void Start()
        {
            isMinion = cb && cb.master && cb.master.minionOwnership && cb.master.minionOwnership.ownerMaster != null;
        }

        public bool CanSpawnSquid()
        {
            if (isMinion && SquidPolyp.minionSquidLimit >= 0)
            {
                return squidList.Count < SquidPolyp.minionSquidLimit;
            }

            if (SquidPolyp.scaleCount)
            {
                return squidList.Count < 2 + (inventory ? inventory.GetItemCount(RoR2Content.Items.Squid) : -2);
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
