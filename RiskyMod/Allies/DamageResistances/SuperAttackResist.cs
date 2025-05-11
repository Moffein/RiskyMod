using RiskyMod.SharedHooks;
using RoR2;
using R2API;
using RiskyMod.Tweaks.CharacterMechanics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace RiskyMod.Allies
{
    public class SuperAttackResist
    {
        public static bool enabled = true;

        public SuperAttackResist()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageNoAttackerActions += AddResist;
            RoR2Application.onLoad += CheckRegigigas;
        }

        private static BodyIndex regiBody;
        private static void CheckRegigigas()
        {
            //RegigigasPlayerBody, not necessary
            regiBody = BodyCatalog.FindBodyIndex("RegigigasBody");
            if (regiBody != BodyIndex.None) TakeDamage.ModifyInitialDamageActions += AddRegiResist;
        }

        private static void AddRegiResist(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (attackerBody.bodyIndex != regiBody || attackerBody.isPlayerControlled) return;

            if ((self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && (self.body.inventory && self.body.inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0))
            {
                if (damageInfo.damageType.damageType.HasFlag(DamageType.AOE)) damageInfo.damage *= 0.3333333333f;   //Restore old AoE resist

                //Cap damage to 90% of ally's combined health
                float effectiveArmor = self.body.armor + self.adaptiveArmorValue;
                float realDamage = damageInfo.damage * (100f / (100f + effectiveArmor));
                float maxDamage = (self.fullCombinedHealth) * 0.9f;
                if (realDamage > maxDamage)
                {
                    damageInfo.damage *= maxDamage / realDamage;
                }
            }
        }

        private static void AddResist(DamageInfo damageInfo, HealthComponent self)
        {
            if (!self.body.isPlayerControlled
                && (damageInfo.HasModdedDamageType(SharedDamageTypes.ResistedByAllies))
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && (self.body.inventory && self.body.inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0))
            {
                damageInfo.procCoefficient *= 0.5f;
                damageInfo.damage *= 0.5f;
            }
        }
    }
}
