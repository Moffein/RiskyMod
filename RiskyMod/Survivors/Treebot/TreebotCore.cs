using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using System;
using RiskyMod.SharedHooks;
using R2API;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace RiskyMod.Survivors.Treebot
{
    public class TreebotCore
    {
        public static bool enabled = true;
        public static bool drillChanges = true;

        public static bool defaultUtilityHeal = true;
        public static bool fruitChanges = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/TreebotBody");
        public TreebotCore()
        {
            if (!enabled) return;
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (drillChanges)
            {
                GameObject drillPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/TreebotMortarRain");
                ProjectileDotZone pdz = drillPrefab.GetComponent<ProjectileDotZone>();
                pdz.overlapProcCoefficient = 0.7f;
            }
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (defaultUtilityHeal)
            {
                SkillDef utilityDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodySonicBoom.asset").WaitForCompletion();
                utilityDef.skillDescriptionToken = "TREEBOT_UTILITY_DESCRIPTION_RISKYMOD";
                new DefaultUtilityHeal();
            }

            new ModifyUtilityForce();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (fruitChanges)
            {
                SkillDef fruitDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyFireFruitSeed.asset").WaitForCompletion();
                fruitDef.skillDescriptionToken = "TREEBOT_SPECIAL_ALT1_DESCRIPTION_RISKYMOD";

                /*Debug.Log("PrepFruitSeed");
                SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Treebot/EntityStates.Treebot.TreebotPrepFruitSeed.asset").WaitForCompletion());
                Debug.Log("\nFireFruitSeed");
                SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Treebot/EntityStates.Treebot.TreebotFireFruitSeed.asset").WaitForCompletion());*/

                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Treebot/EntityStates.Treebot.TreebotFireFruitSeed.asset", "baseDuration", "0.5");//1 in vanilla

                GameObject fruitProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/TreebotFruitSeedProjectile");
                ProjectileImpactExplosion pie = fruitProjectile.GetComponent<ProjectileImpactExplosion>();
                pie.blastRadius = 6f;

                new ModifyFruitPickup();
                new DropFruitOnHit();

                RecalculateStatsAPI.GetStatCoefficients += FruitBuff;
                SharedHooks.OnHitEnemy.OnHitAttackerActions += FruitHealOnHit;

                //Fruit on-death nullref is located in Fixes section
            }
        }

        private void FruitBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.Fruiting))
            {
                args.moveSpeedReductionMultAdd += 0.8f;
            }
        }

        private void FruitHealOnHit(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (victimBody.HasBuff(RoR2Content.Buffs.Fruiting) && attackerBody.healthComponent)
            {
                attackerBody.healthComponent.Heal(damageInfo.damage * 0.05f, default);
            }
        }
    }
}
