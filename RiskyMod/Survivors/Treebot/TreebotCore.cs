using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using System;
using RiskyMod.SharedHooks;
using R2API;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;

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
                sk.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "TREEBOT_UTILITY_DESCRIPTION_RISKYMOD";
                //sk.utility.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_SONIC_BOOM" };

                //sk.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "TREEBOT_UTILITY_ALT1_DESCRIPTION_RISKYMOD";
                //sk.utility.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_SONIC_BOOM", "KEYWORD_WEAK" };
                new DefaultUtilityHeal();
            }

            new ModifyUtilityForce();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (fruitChanges)
            {
                sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "TREEBOT_SPECIAL_ALT1_DESCRIPTION_RISKYMOD";

                /*Debug.Log("PrepFruitSeed");
                SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Treebot/EntityStates.Treebot.TreebotPrepFruitSeed.asset").WaitForCompletion());
                Debug.Log("\nFireFruitSeed");
                SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Treebot/EntityStates.Treebot.TreebotFireFruitSeed.asset").WaitForCompletion());*/

                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Treebot/EntityStates.Treebot.TreebotFireFruitSeed.asset", "baseDuration", "0.5");//1 in vanilla

                GameObject fruitProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/TreebotFruitSeedProjectile");
                ProjectileImpactExplosion pie = fruitProjectile.GetComponent<ProjectileImpactExplosion>();
                pie.blastRadius = 4f;

                new ModifyFruitPickup();
                new DropFruitOnHit();

                RecalculateStatsAPI.GetStatCoefficients += FruitBuff;

                //Fruit on-death nullref is located in Fixes section
            }
        }

        private static void FruitBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.Fruiting))
            {
                args.moveSpeedReductionMultAdd += 0.8f;
            }
        }
    }
}
