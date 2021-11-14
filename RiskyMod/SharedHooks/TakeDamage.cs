using R2API;
using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.Equipment;
using RiskyMod.Items.Uncommon;
using RiskyMod.MonoBehaviours;
using RiskyMod.Tweaks;
using RoR2;
using RoR2.Audio;
using RoR2.CharacterAI;
using RoR2.Orbs;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class TakeDamage
    {
        public delegate void OnHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost);
        public static OnHpLost HandleOnHpLostActions = HandleOnHpLostMethod;
        private static void HandleOnHpLostMethod(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost) { }

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            float oldHP = self.combinedHealth;
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
                            //Convert to proc chain when custom proc chains are a thing.
                            //Currently has no protection against self-proc.
                            if (self.body != attackerBody && self.combinedHealth >= self.fullCombinedHealth * 0.9f)
                            {
                                int crowbarCount = attackerInventory.GetItemCount(RoR2Content.Items.Crowbar);
                                if (crowbarCount > 0)
                                {
                                    damageInfo.damage *= 1f + 0.5f * crowbarCount;
                                    EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.crowbarImpactEffectPrefab, damageInfo.position, -damageInfo.force, true);
                                    damageInfo.AddModdedDamageType(Crowbar.crowbarDamage);
                                }
                            }
                        }
                    }
                }
            }

            if (FixSlayer.enabled && (damageInfo.damageType & DamageType.BonusToLowHealth) > DamageType.Generic)
            {
                damageInfo.damageType &= ~DamageType.BonusToLowHealth;
                damageInfo.damage *= Mathf.Lerp(3f, 1f, self.combinedHealthFraction);
            }

            orig(self, damageInfo);

            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (self.alive)
                {
                    if (self.body)
                    {
                        /*if (HarvesterScythe.enabled && self.body.HasBuff(HarvesterScythe.scytheBuff))
                        {
                            self.body.ClearTimedBuffs(HarvesterScythe.scytheBuff);
                        }*/
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

                        Inventory inventory = self.body.inventory;
                        if (inventory)
                        {
                            float percentHPLost = (oldHP - self.combinedHealth) / self.fullCombinedHealth * 100f;
                            HandleOnHpLostActions.Invoke(damageInfo, self, inventory, percentHPLost);

                            //This should happen after OnHpLost
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
