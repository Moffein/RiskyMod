using RoR2;
using R2API;
using UnityEngine;
using RoR2.Skills;
using System.Runtime.CompilerServices;
using EntityStates.RiskyMod.Croco;
using EntityStates;
using UnityEngine.Networking;
using RoR2.Orbs;

namespace RiskyMod.Survivors.Croco
{
    public class CrocoCore
    {
        public static bool enabled = true;
        public static bool gameplayRework = true;

        public CrocoCore()
        {
            if (!enabled) return;
            new BlightStack();
            new BiggerMeleeHitbox();
            new ExtendSpawnInvuln();
            new RemovePoisonDamageCap();
            if (gameplayRework)
            {
                new RegenRework();
                ModifyStats(RoR2Content.Survivors.Croco.bodyPrefab.GetComponent<CharacterBody>());
                ModifySkills(RoR2Content.Survivors.Croco.bodyPrefab.GetComponent<SkillLocator>());
            }
        }

        private void ModifyStats(CharacterBody cb)
        {
            cb.baseDamage = 12f;
            cb.levelDamage = cb.baseDamage * 0.2f;
        }

        //Is the playstyle still feeling too basic?
        private void ModifySkills(SkillLocator sk)
        {
            ModifyPassives(sk);
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPassives(SkillLocator sk)
        {
            GenericSkill passives = RoR2Content.Survivors.Croco.bodyPrefab.GetComponent<GenericSkill>();    //Passive is at the top
            passives.skillFamily.variants[0].skillDef.skillNameToken = "CROCO_PASSIVE_NAME_RISKYMOD";
            passives.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_PASSIVE_DESCRIPTION_RISKYMOD";
            passives.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_POISON_RISKYMOD", "KEYWORD_BLIGHT_RISKYMOD" };

            passives.skillFamily.variants[1].skillDef.skillNameToken = "CROCO_PASSIVE_ALT_NAME_RISKYMOD";
            passives.skillFamily.variants[1].skillDef.skillDescriptionToken = "CROCO_PASSIVE_ALT_DESCRIPTION_RISKYMOD";
            passives.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_POISON_RISKYMOD", "KEYWORD_BLIGHT_RISKYMOD" };
            new ModifyPassives();
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "damageCoefficient", "2");
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "comboFinisherDamageCoefficient", "5");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "baseDuration", "1.2");

            sk.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_PRIMARY_DESCRIPTION_RISKYMOD";
            sk.primary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_POISON_RISKYMOD", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            new ModifyM1();
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_SECONDARY_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD" };
            sk.secondary.skillFamily.variants[0].skillDef.baseRechargeInterval = 3f;
            new ModifyM2Spit();

            //I hate how this has so many keywords.
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Bite", "damageCoefficient", "3.6");
            sk.secondary.skillFamily.variants[1].skillDef.skillDescriptionToken = "CROCO_SECONDARY_ALT_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_SLAYER", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            new ModifyM2Bite();
        }

        //Both Shifts can Blight, otherwise the default ends up being objectively better.
        private void ModifyUtilities(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Leap", "blastDamageCoefficient", "3.2");
            sk.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_UTILITY_DESCRIPTION_RISKYMOD";
            sk.utility.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_STUNNING"};

            //Check to see if the cooldown reset is too crazy.
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.ChainableLeap", "blastDamageCoefficient", "3.2");
            sk.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "CROCO_UTILITY_ALT1_DESCRIPTION_RISKYMOD";
            sk.utility.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_STUNNING"};
            sk.utility.skillFamily.variants[1].skillDef.baseMaxStock = 1;
            sk.utility.skillFamily.variants[1].skillDef.baseRechargeInterval = 6f;
            new ModifyShift();
        }

        //Watch out to make sure this isn't making a worse autoplay situation than the original.
            //Seems strong, but isn't something you can solely rely on to clear the map like Vanilla poison.
        private void ModifySpecials(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireDiseaseProjectile", "damageCoefficient", "1");
            sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_SPECIAL_DESCRIPTION_RISKYMOD";
            sk.special.skillFamily.variants[0].skillDef.keywordTokens = new string[] {};
            new ModifySpecial();

            if (RiskyMod.ScepterPluginLoaded)
            {
                SetupScepter(sk);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter(SkillLocator sk)
        {
            SkillDef orig = sk.special.skillFamily.variants[0].skillDef;

            SkillDef diseaseScepterDef = ScriptableObject.CreateInstance<SkillDef>();
            diseaseScepterDef.activationState = new SerializableEntityStateType(typeof(FireDiseaseProjectileScepter));
            diseaseScepterDef.activationStateMachineName = orig.activationStateMachineName;
            diseaseScepterDef.baseMaxStock = 1;
            diseaseScepterDef.baseRechargeInterval = 10f;
            diseaseScepterDef.beginSkillCooldownOnSkillEnd = false;
            diseaseScepterDef.canceledFromSprinting = false;
            diseaseScepterDef.cancelSprintingOnActivation = orig.cancelSprintingOnActivation;
            diseaseScepterDef.dontAllowPastMaxStocks = true;
            diseaseScepterDef.forceSprintDuringState = false;
            diseaseScepterDef.fullRestockOnAssign = true;
            diseaseScepterDef.icon = AncientScepter.Assets.SpriteAssets.CrocoDisease2;
            diseaseScepterDef.interruptPriority = orig.interruptPriority;
            diseaseScepterDef.isCombatSkill = orig.isCombatSkill;
            diseaseScepterDef.keywordTokens = orig.keywordTokens;
            diseaseScepterDef.mustKeyPress = orig.mustKeyPress;
            diseaseScepterDef.rechargeStock = 1;
            diseaseScepterDef.requiredStock = 1;
            diseaseScepterDef.resetCooldownTimerOnUse = orig.resetCooldownTimerOnUse;
            diseaseScepterDef.skillDescriptionToken = "CROCO_SPECIAL_DESCRIPTION_SCEPTER_RISKYMOD";
            diseaseScepterDef.skillName = "DiseaseScepter";
            diseaseScepterDef.skillNameToken = "CROCO_SPECIAL_NAME_SCEPTER_RISKYMOD";
            diseaseScepterDef.stockToConsume = 1;

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(diseaseScepterDef, "CrocoBody", SkillSlot.Special, 0);
        }

    }
}
