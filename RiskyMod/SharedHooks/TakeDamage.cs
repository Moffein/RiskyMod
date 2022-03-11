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
        public static OnPercentHpLost HandleOnPercentHpLostActions;

        public delegate void OnHpLostAttacker(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory inventory, float hpLost);
        public static OnHpLostAttacker OnHpLostAttackerActions;

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
                    if (ModifyInitialDamageActions != null) ModifyInitialDamageActions.Invoke(damageInfo, self, attackerBody);
                    attackerInventory = attackerBody.inventory;
                    if (attackerInventory)
                    {
                        if (ModifyInitialDamageInventoryActions != null) ModifyInitialDamageInventoryActions.Invoke(damageInfo, self, attackerBody, attackerInventory);
                    }
                }
            }

            orig(self, damageInfo);

            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (self.alive)
                {
                    if (OnDamageTakenActions != null) OnDamageTakenActions.Invoke(damageInfo, self);

                    if (damageInfo.attacker)
                    {
                        if (attackerBody)
                        {
                            if (OnDamageTakenAttackerActions != null) OnDamageTakenAttackerActions.Invoke(damageInfo, self, attackerBody);
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
                                if (OnHpLostAttackerActions != null)  OnHpLostAttackerActions.Invoke(damageInfo, self, attackerBody, inventory, totalHPLost);
                            }
                            float percentHPLost = totalHPLost / self.fullCombinedHealth * 100f;
                            if (HandleOnPercentHpLostActions != null) HandleOnPercentHpLostActions.Invoke(damageInfo, self, inventory, percentHPLost);
                        }
                    }
                    if (TakeDamageEndActions != null) TakeDamageEndActions.Invoke(damageInfo, self);
                }
            }
        }
    }
}
