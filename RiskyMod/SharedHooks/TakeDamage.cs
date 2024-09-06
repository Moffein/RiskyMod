using R2API;
using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.Uncommon;
using RiskyMod.Tweaks;
using RoR2;
using RoR2.Audio;
using RoR2.CharacterAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class TakeDamage
    {
        public delegate void OnPercentHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost);
        public static OnPercentHpLost OnPercentHpLostActions;

        public delegate void OnHpLostAttacker(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory inventory, float hpLost);
        public static OnHpLostAttacker OnHpLostAttackerActions;

        public delegate void ModifyInitialDamageNoAttacker(DamageInfo damageInfo, HealthComponent self);
        public static ModifyInitialDamageNoAttacker ModifyInitialDamageNoAttackerActions;

        public delegate void ModifyInitialDamage(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody);
        public static ModifyInitialDamage ModifyInitialDamageActions;

        public delegate void ModifyInitialDamageInventory(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory attackerInventory);
        public static ModifyInitialDamageInventory ModifyInitialDamageInventoryActions;

        public delegate void OnDamageTaken(DamageInfo damageInfo, HealthComponent self);
        public static OnDamageTaken OnDamageTakenActions;

        public delegate void OnDamageTakenAttacker(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody);
        public static OnDamageTakenAttacker OnDamageTakenAttackerActions;

        public delegate void TakeDamageEnd(DamageInfo damageInfo, HealthComponent self);
        public static TakeDamageEnd TakeDamageEndActions;

        public static List<BodyIndex> distractOnHitBodies = new List<BodyIndex>();
        public static void DistractOnHit(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            //Based on https://github.com/DestroyedClone/PoseHelper/blob/master/HighPriorityAggroTest/HPATPlugin.cs
            if (!self.body.isChampion && self.body.master && self.body.master.aiComponents.Length > 0 && distractOnHitBodies.Contains(attackerBody.bodyIndex))
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

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            float oldHP = self.combinedHealth;
            CharacterBody attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            Inventory attackerInventory = attackerBody ? attackerBody.inventory : null;

            ModifyInitialDamageNoAttackerActions?.Invoke(damageInfo, self);
            if (attackerBody)
            {
                ModifyInitialDamageActions?.Invoke(damageInfo, self, attackerBody);
                if (attackerInventory) ModifyInitialDamageInventoryActions?.Invoke(damageInfo, self, attackerBody, attackerInventory);
            }

            orig(self, damageInfo);

            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (self.alive)
                {
                    OnDamageTakenActions?.Invoke(damageInfo, self);

                    if (damageInfo.attacker)
                    {
                        if (attackerBody)
                        {
                            OnDamageTakenAttackerActions?.Invoke(damageInfo, self, attackerBody);
                        }
                    }

                    Inventory inventory = self.body.inventory;
                    if (inventory)
                    {
                        float totalHPLost = oldHP - self.combinedHealth;
                        if (totalHPLost > 0f)
                        {
                            if (attackerBody)
                            {
                                OnHpLostAttackerActions?.Invoke(damageInfo, self, attackerBody, inventory, totalHPLost);
                            }
                            float percentHPLost = totalHPLost / self.fullCombinedHealth;
                            percentHPLost = Mathf.Max(percentHPLost, damageInfo.damage / self.fullCombinedHealth);   //Use this to emulate the actual Early Access Stealthkit Behavior
                            OnPercentHpLostActions?.Invoke(damageInfo, self, inventory, 100f * percentHPLost);
                        }
                    }
                    TakeDamageEndActions?.Invoke(damageInfo, self);
                }
            }
        }
    }
}
