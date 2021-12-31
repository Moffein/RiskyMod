using R2API;
using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.Uncommon;
using RiskyMod.Tweaks;
using RoR2;
using RoR2.Audio;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class TakeDamage
    {
        public delegate void OnPercentHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost);
        public static OnPercentHpLost HandleOnPercentHpLostActions = HandleOnPercentHpLostMethod;
        private static void HandleOnPercentHpLostMethod(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost) { }

        public delegate void OnHpLostAttacker(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory inventory, float hpLost);
        public static OnHpLostAttacker HandleOnHpLostAttackerActions = HandleOnHpLostAttackerMethod;
        private static void HandleOnHpLostAttackerMethod(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory inventory, float hpLost) { }

        public delegate void ModifyInitialDamage(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody);
        public static ModifyInitialDamage ModifyInitialDamageActions = ModifyInitialDamageMethod;
        private static void ModifyInitialDamageMethod(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody) { }

        public delegate void ModifyInitialDamageInventory(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory attackerInventory);
        public static ModifyInitialDamageInventory ModifyInitialDamageInventoryActions = ModifyInitialDamageInventoryMethod;
        private static void ModifyInitialDamageInventoryMethod(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory attackerInventory) { }

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
                    ModifyInitialDamageActions.Invoke(damageInfo, self, attackerBody);
                    attackerInventory = attackerBody.inventory;
                    if (attackerInventory)
                    {
                        ModifyInitialDamageInventoryActions.Invoke(damageInfo, self, attackerBody, attackerInventory);
                    }
                }
            }

            orig(self, damageInfo);

            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (self.alive)
                {
                    if (self.body)
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

                        Inventory inventory = self.body.inventory;
                        if (inventory)
                        {
                            float totalHPLost = oldHP - self.combinedHealth;
                            if (totalHPLost > 0f)
                            {
                                if (attackerBody)
                                {
                                    HandleOnHpLostAttackerActions.Invoke(damageInfo, self, attackerBody, inventory, totalHPLost);
                                }
                                float percentHPLost = totalHPLost / self.fullCombinedHealth * 100f;
                                HandleOnPercentHpLostActions.Invoke(damageInfo, self, inventory, percentHPLost);
                            }

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
