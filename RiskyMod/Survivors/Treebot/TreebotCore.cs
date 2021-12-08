using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using System;
using RiskyMod.SharedHooks;
using R2API;
using MonoMod.Cil;

namespace RiskyMod.Survivors.Treebot
{
    public class TreebotCore
    {
        public static bool enabled = true;
        public static bool enableSecondarySkillChanges = true;
        public static bool enableUtilitySkillChanges = true;
        public static bool enableSpecialSkillChanges = true;
        public TreebotCore()
        {
            if (!enabled) return;
            ModifySkills(RoR2Content.Survivors.Treebot.bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (!enableSecondarySkillChanges) return;

            GameObject drillPrefab = Resources.Load<GameObject>("prefabs/projectiles/TreebotMortarRain");
            ProjectileDotZone pdz = drillPrefab.GetComponent<ProjectileDotZone>();
            pdz.overlapProcCoefficient = 0.7f;

            //Was originally planning to make it have the same type of hitbox extension and downwards force as Arrow Rain but it didn't play well.
            //No projectile rotation makes the force feel less flexible, and causes the extended hitbox to poke through weird places when used on walls.
            //Downwards force would push flying enemies out of the hitbox.
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (!enableUtilitySkillChanges) return;

            sk.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "TREEBOT_UTILITY_DESCRIPTION_RISKYMOD";
            sk.utility.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_SONIC_BOOM"};

            sk.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "TREEBOT_UTILITY_ALT1_DESCRIPTION_RISKYMOD";
            sk.utility.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_SONIC_BOOM", "KEYWORD_WEAK" };

            new SwapUtilityEffects();
            new ModifyUtilityForce();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (!enableSpecialSkillChanges) return;

            sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "TREEBOT_SPECIAL_ALT1_DESCRIPTION_RISKYMOD";

            GameObject fruitProjectile = Resources.Load<GameObject>("prefabs/projectiles/TreebotFruitSeedProjectile");
            ProjectileImpactExplosion pie = fruitProjectile.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = 4f;

            new ModifyFruitPickup();
            new DropFruitOnHit();

            GetStatsCoefficient.HandleStatsActions += FruitBuff;

            //Fruit on-death nullref is located in Fixes section
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
