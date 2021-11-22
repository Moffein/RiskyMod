using EntityStates;
using EntityStates.RiskyMod.Bandit2;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class Bandit2Core
    {
        public static DamageAPI.ModdedDamageType CanBackstab;
        public static bool enabled = true;

        public static bool enablePassiveSkillChanges = true;
        public static bool enablePrimarySkillChanges = true;
        public static bool enableSecondarySkillChanges = true;
        public static bool enableUtilitySkillChanges = true;
        public static bool enableSpecialSkillChanges = true;


        public Bandit2Core()
        {
            if (!enabled) return;

            CanBackstab = DamageAPI.ReserveDamageType();
            RoR2Content.Survivors.Bandit2.bodyPrefab.AddComponent<PassiveComponent>();    //Move to passive section

            new BanditSpecialGracePeriod();
            ModifySkills(RoR2Content.Survivors.Bandit2.bodyPrefab.GetComponent<SkillLocator>());
        }
        private void ModifySkills(SkillLocator sk)
        {
            ModifyPassives(sk);
            ModifyPrimaries(sk);
            ModifyUtilities(sk);
        }

        private void ModifyPassives(SkillLocator sk)
        {
            if (!enablePassiveSkillChanges) return;
            new BackstabRework();
            
            //Todo: add Quickdraw option
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (!enablePrimarySkillChanges) return;

            LoadoutAPI.AddSkill(typeof(EnterReload));
            LoadoutAPI.AddSkill(typeof(Reload));

            LoadoutAPI.AddSkill(typeof(FirePrimaryShotgun));
            ReloadSkillDef shotgunDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
            shotgunDef.activationState = new SerializableEntityStateType(typeof(FirePrimaryShotgun));
            shotgunDef.activationStateMachineName = "Weapon";
            shotgunDef.baseMaxStock = 4;
            shotgunDef.baseRechargeInterval = 0f;
            shotgunDef.beginSkillCooldownOnSkillEnd = false;
            shotgunDef.canceledFromSprinting = false;
            shotgunDef.dontAllowPastMaxStocks = true;
            shotgunDef.forceSprintDuringState = false;
            shotgunDef.fullRestockOnAssign = true;
            shotgunDef.icon = sk.primary._skillFamily.variants[0].skillDef.icon;
            shotgunDef.interruptPriority = InterruptPriority.Skill;
            shotgunDef.isCombatSkill = true;
            shotgunDef.keywordTokens = new string[] { };
            shotgunDef.mustKeyPress = false;
            shotgunDef.cancelSprintingOnActivation = true;
            shotgunDef.rechargeStock = 0;
            shotgunDef.requiredStock = 1;
            shotgunDef.skillName = "FireShotgun";
            shotgunDef.skillNameToken = "BANDIT2_PRIMARY_NAME";
            shotgunDef.skillDescriptionToken = "BANDIT2_PRIMARY_DESC_RISKYMOD";
            shotgunDef.stockToConsume = 1;
            shotgunDef.graceDuration = 0.4f;
            shotgunDef.reloadState = new SerializableEntityStateType(typeof(EnterReload));
            shotgunDef.reloadInterruptPriority = InterruptPriority.Any;
            LoadoutAPI.AddSkillDef(shotgunDef);
            Skills.Burst = shotgunDef;
            sk.primary._skillFamily.variants[0].skillDef = Skills.Burst;

            LoadoutAPI.AddSkill(typeof(FirePrimaryRifle));
            ReloadSkillDef rifleDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
            rifleDef.activationState = new SerializableEntityStateType(typeof(FirePrimaryRifle));
            rifleDef.activationStateMachineName = "Weapon";
            rifleDef.baseMaxStock = 4;
            rifleDef.baseRechargeInterval = 0f;
            rifleDef.beginSkillCooldownOnSkillEnd = false;
            rifleDef.canceledFromSprinting = false;
            rifleDef.dontAllowPastMaxStocks = true;
            rifleDef.forceSprintDuringState = false;
            rifleDef.fullRestockOnAssign = true;
            rifleDef.icon = sk.primary._skillFamily.variants[1].skillDef.icon;
            rifleDef.interruptPriority = InterruptPriority.Skill;
            rifleDef.isCombatSkill = true;
            rifleDef.keywordTokens = new string[] { };
            rifleDef.mustKeyPress = false;
            rifleDef.cancelSprintingOnActivation = true;
            rifleDef.rechargeStock = 0;
            rifleDef.requiredStock = 1;
            rifleDef.skillName = "FireRifle";
            rifleDef.skillNameToken = "BANDIT2_PRIMARY_ALT_NAME";
            rifleDef.skillDescriptionToken = "BANDIT2_PRIMARY_ALT_DESC_RISKYMOD";
            rifleDef.stockToConsume = 1;
            rifleDef.graceDuration = 0.4f;
            rifleDef.reloadState = new SerializableEntityStateType(typeof(EnterReload));
            rifleDef.reloadInterruptPriority = InterruptPriority.Any;
            LoadoutAPI.AddSkillDef(rifleDef);
            Skills.Blast = rifleDef;
            sk.primary._skillFamily.variants[1].skillDef = Skills.Blast;
        }
        private void ModifyUtilities(SkillLocator sk)
        {
            if (!enableUtilitySkillChanges) return;

            LoadoutAPI.AddSkill(typeof(ThrowSmokebomb));
            LoadoutAPI.AddSkill(typeof(StealthMode));
            SkillDef stealthDef = SkillDef.CreateInstance<SkillDef>();
            stealthDef.activationState = new SerializableEntityStateType(typeof(ThrowSmokebomb));
            stealthDef.activationStateMachineName = "Stealth";
            stealthDef.baseMaxStock = 1;
            stealthDef.baseRechargeInterval = 11f;
            stealthDef.beginSkillCooldownOnSkillEnd = false;
            stealthDef.canceledFromSprinting = false;
            stealthDef.forceSprintDuringState = true;
            stealthDef.dontAllowPastMaxStocks = true;
            stealthDef.fullRestockOnAssign = true;
            stealthDef.icon = sk.utility._skillFamily.variants[0].skillDef.icon;
            stealthDef.interruptPriority = InterruptPriority.Skill;
            stealthDef.isCombatSkill = false;
            stealthDef.keywordTokens = new string[] { };
            stealthDef.mustKeyPress = false;
            stealthDef.cancelSprintingOnActivation = true;
            stealthDef.rechargeStock = 1;
            stealthDef.requiredStock = 1;
            stealthDef.skillName = "Stealth";
            stealthDef.skillNameToken = "BANDIT2_UTILITY_NAME";
            stealthDef.skillDescriptionToken = "BANDIT2_UTILITY_DESCRIPTION";
            stealthDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(stealthDef);
            Skills.Smokebomb = stealthDef;

            sk.utility._skillFamily.variants[0].skillDef = Skills.Smokebomb;
        }
    }

    public class Skills
    {
        public static SkillDef Backstab;    //Backstabs are crits (no AOE allowed except for Cloak)
        public static SkillDef Quickdraw;   //

        public static SkillDef Gunslinger;
        public static SkillDef Desperado;

        public static ReloadSkillDef Blast;
        public static ReloadSkillDef Burst;

        public static SkillDef Knife;
        public static SkillDef ThrowKnife;
        public static SkillDef Dynamite;

        public static SkillDef Smokebomb;

        public static SkillDef LightsOut;
        public static SkillDef RackEmUp;
    }
}
