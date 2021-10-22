using R2API;
using Risky_ItemTweaks.Items.Boss;
using Risky_ItemTweaks.Items.Common;
using Risky_ItemTweaks.Items.Uncommon;
using Risky_ItemTweaks.MonoBehaviours;
using RoR2;
using RoR2.Audio;
using RoR2.CharacterAI;
using RoR2.Orbs;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Risky_ItemTweaks.SharedHooks
{
    public class TakeDamage
    {
        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            float oldHP = self.health;
            CharacterBody attackerBody = null;
            Inventory attackerInventory = null;
            if (damageInfo.attacker)
            {
                attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    attackerInventory = attackerBody.inventory;
                    if (attackerInventory)
                    {
                        if (Crowbar.enabled)
                        {
                            //Conver to proc chain when custom proc chains are a thing.
                            //Currently has no protection against self-proc.
                            if (self.combinedHealth >= self.fullCombinedHealth * 0.9f)
                            {
                                int crowbarCount = attackerInventory.GetItemCount(RoR2Content.Items.Crowbar);
                                if (crowbarCount > 0)
                                {
                                    damageInfo.damage *= 1f + 0.5f * crowbarCount;
                                    EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.crowbarImpactEffectPrefab, damageInfo.position, -damageInfo.force, true);
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo);

            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (self.alive)
                {
                    if (damageInfo.attacker)
                    {
                        if (attackerBody)
                        {
                            if (SquidPolyp.enabled)
                            {
                                //Squid Turrets are guaranteed to draw aggro upon dealing damage.
                                //Based on https://github.com/DestroyedClone/PoseHelper/blob/master/HighPriorityAggroTest/HPATPlugin.cs
                                if (damageInfo.attacker.name == "SquidTurretBody(Clone)")
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
                        }
                    }

                    if (self.body)
                    {
                        Inventory inventory = self.body.inventory;
                        if (inventory)
                        {
                            float percentHPLost = (oldHP - self.health) / self.fullCombinedHealth * 100f;
                            if (self.body.master)
                            {
                                //Basing this off of https://riskofrain2.fandom.com/wiki/Old_War_Stealthkit Version History
                                if (Stealthkit.enabled && !self.body.HasBuff(RoR2Content.Buffs.Cloak))
                                {
                                    int stealthkitCount = inventory.GetItemCount(RoR2Content.Items.Phasing);
                                    if (stealthkitCount > 0)
                                    {
                                        if (percentHPLost > 0f)
                                        {
                                            if (Util.CheckRoll(percentHPLost, self.body.master))
                                            {
                                                float buffDuration = 1.5f + stealthkitCount * 1.5f;
                                                self.body.AddTimedBuff(RoR2Content.Buffs.Cloak, buffDuration);
                                                self.body.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, buffDuration);
                                                EffectManager.SpawnEffect(Stealthkit.effectPrefab, new EffectData
                                                {
                                                    origin = self.body.transform.position,
                                                    rotation = Quaternion.identity
                                                }, true);
                                            }
                                        }
                                    }
                                }
                                if (SquidPolyp.enabled)
                                {
                                    int polypCount = inventory.GetItemCount(RoR2Content.Items.Squid);
                                    if (polypCount > 0)
                                    {
                                        if (percentHPLost > 0f)
                                        {
                                            if (Util.CheckRoll(percentHPLost, self.body.master))
                                            {
                                                SquidMinionManagerComponent sq = self.gameObject.GetComponent<SquidMinionManagerComponent>();
                                                if (!sq)
                                                {
                                                    sq = self.gameObject.AddComponent<SquidMinionManagerComponent>();
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
                                                    DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
                                                    directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
                                                    {
                                                        if (!result.success)
                                                        {
                                                            return;
                                                        }
                                                        CharacterMaster component6 = result.spawnedInstance.GetComponent<CharacterMaster>();
                                                        component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 30);
                                                        component6.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, 10 * (polypCount - 1));
                                                        if (self.itemCounts.invadingDoppelganger > 0)
                                                        {
                                                            //Prevent turrets from hitscanning players to death at full damage
                                                            component6.inventory.GiveItem(RoR2Content.Items.InvadingDoppelganger);
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
                            if (Razorwire.enabled)
                            {
                                int thornCount = inventory.GetItemCount(RoR2Content.Items.Thorns);
                                if (thornCount > 0 && !damageInfo.procChainMask.HasProc(ProcType.Thorns))
                                {
                                    float hpLostLerp = percentHPLost / 100f;
                                    int targetCount = 3 + 2 * thornCount;
                                    bool isVengeanceClone = inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0; //In case some other mod tries to mess with HealthComponent's itemcount

                                    bool isCrit = self.body.RollCrit();
                                    float damageValue = Mathf.Lerp(0.84f, 4.2f, hpLostLerp) * self.body.damage;
                                    float proc = Mathf.Lerp(0.1f, 0.5f, hpLostLerp);
                                    float radius = Mathf.Lerp(16f, 40f, hpLostLerp);

                                    TeamIndex teamIndex2 = self.body.teamComponent.teamIndex;
                                    HurtBox[] hurtBoxes = new SphereSearch
                                    {
                                        origin = damageInfo.position,
                                        radius = radius,
                                        mask = LayerIndex.entityPrecise.mask,
                                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
                                    }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex2)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
                                    for (int k = 0; k < Mathf.Min(targetCount, hurtBoxes.Length); k++)
                                    {
                                        LightningOrb lightningOrb = new LightningOrb();
                                        lightningOrb.attacker = self.gameObject;
                                        lightningOrb.bouncedObjects = null;
                                        lightningOrb.bouncesRemaining = 0;
                                        lightningOrb.damageCoefficientPerBounce = 1f;
                                        lightningOrb.damageColorIndex = DamageColorIndex.Item;
                                        lightningOrb.damageValue = damageValue;
                                        lightningOrb.isCrit = isCrit;
                                        lightningOrb.lightningType = LightningOrb.LightningType.RazorWire;
                                        lightningOrb.origin = damageInfo.position;
                                        lightningOrb.procChainMask = default(ProcChainMask);
                                        lightningOrb.procChainMask.AddProc(ProcType.Thorns);
                                        lightningOrb.procCoefficient = isVengeanceClone ? 0f : (Risky_ItemTweaks.disableProcChains ? proc : 0.5f);
                                        lightningOrb.range = 0f;
                                        lightningOrb.teamIndex = teamIndex2;
                                        lightningOrb.target = hurtBoxes[k];
                                        OrbManager.instance.AddOrb(lightningOrb);
                                    }
                                }
                            }
                            if (Planula.enabled)
                            {
                                int planulaCount = inventory.GetItemCount(RoR2Content.Items.ParentEgg);
                                if (planulaCount > 0)
                                {
                                    self.Heal(planulaCount * 15f, default(ProcChainMask), true);
                                    EntitySoundManager.EmitSoundServer(Resources.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseParentEggHeal").index, self.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
