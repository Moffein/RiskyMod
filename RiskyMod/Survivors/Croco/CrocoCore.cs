using RoR2;
using R2API;
using UnityEngine;
using RoR2.Skills;
using System.Runtime.CompilerServices;
using EntityStates.RiskyMod.Croco;
using EntityStates;
using UnityEngine.Networking;
using RoR2.Orbs;
using RiskyMod.Content;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco
{
    public class CrocoCore
    {
        public static bool enabled = true;
        public static bool gameplayRework = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody");

        //I don't like how some of these values are used on-the-fly while others are only used during setup.
        //Add a Set method to handle live value changing?
        public static class Cfg
        {
            public static bool enabled = false; //If set to false, will only use the needed values from this. If set to true, will load all values.
            public static class Stats
            {
                public static bool enabled = true;
                public static float health = 160f;
                public static float armor = 20f;
                public static float damage = 12f;
                public static float regen = 2.5f;
            }
            public static class Regenerative
            {
                public static float healFraction = 0.1f;
                public static float healDuration = 3f;
            }

            public static class Passives
            {
                public static float baseDoTDuration = 6f;
                public static float virulentDurationMult = 1.8f;
                public static float contagionSpreadRange = 30f;
            }

            public static class Skills
            {

                public static class ViciousWounds
                {
                    public static float damageCoefficient = 2f;
                    public static float finisherDamageCoefficient = 4f;
                    public static float baseDuration = 1.2f;
                }

                public static class Neurotoxin
                {
                    public static float damageCoefficient = 2.4f;
                    public static float cooldown = 2f;
                }

                public static class Bite
                {
                    public static float damageCoefficient = 3.6f;
                    public static float cooldown = 2f;
                    public static float healFractionOnKill = 0.08f;
                }

                public static class CausticLeap
                {
                    public static float cooldown = 6f;
                    public static float damageCoefficient = 3.2f;
                    public static float acidProcCoefficient = 0.5f;
                }

                public static class FrenziedLeap
                {
                    public static float cooldown = 6f;
                    public static float cooldownReduction = 1f;
                    public static float damageCoefficient = 5.5f;
                }

                public static class Epidemic
                {
                    public static float cooldown = 10f;
                    public static int baseTickCount = 7;    //Initial hit is 1 tick
                    public static float timeBetweenTicks = 0.5f;
                    public static float damageCoefficient = 1f;
                    public static float procCoefficient = 0.5f;
                    public static float spreadRange = 30f;
                }
            }
        }

        public CrocoCore()
        {
            if (!enabled) return;
            new ShiftAirControl();
            new BlightStack();
            new BiggerMeleeHitbox();
            new BiggerLeapHitbox();
            new ExtendSpawnInvuln();
            new RemovePoisonDamageCap();
            if (gameplayRework)
            {
                new RegenRework();
                ModifyStats(bodyPrefab.GetComponent<CharacterBody>());
                ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
            }
        }

        private void ModifyStats(CharacterBody cb)
        {

            if (CrocoCore.Cfg.enabled)
            {
                if (CrocoCore.Cfg.Stats.enabled)
                {
                    cb.baseDamage = Cfg.Stats.damage;
                    cb.levelDamage = cb.baseDamage * 0.2f;

                    cb.baseMaxHealth = Cfg.Stats.health;
                    cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;

                    cb.baseRegen = Cfg.Stats.regen;
                    cb.levelRegen = cb.baseRegen * 0.2f;

                    cb.baseArmor = Cfg.Stats.armor;
                }
            }
            else
            {
                cb.baseDamage = Cfg.Stats.damage;
                cb.levelDamage = cb.baseDamage * 0.2f;
            }
        }

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
            GenericSkill passives = bodyPrefab.GetComponent<GenericSkill>();    //Passive is at the top
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
            
            if (CrocoCore.Cfg.enabled)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "damageCoefficient", SneedUtils.SneedUtils.FloatToString(CrocoCore.Cfg.Skills.ViciousWounds.damageCoefficient));
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "comboFinisherDamageCoefficient", SneedUtils.SneedUtils.FloatToString(CrocoCore.Cfg.Skills.ViciousWounds.finisherDamageCoefficient));
            }
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "baseDuration", SneedUtils.SneedUtils.FloatToString(CrocoCore.Cfg.Skills.ViciousWounds.baseDuration));

            SkillDef primaryDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoSlash.asset").WaitForCompletion();
            primaryDef.skillDescriptionToken = "CROCO_PRIMARY_DESCRIPTION_RISKYMOD";
            primaryDef.keywordTokens = new string[] { "KEYWORD_POISON_RISKYMOD", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            new ModifyM1();
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            SkillDef spitDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoSpit.asset").WaitForCompletion();
            spitDef.skillDescriptionToken = "CROCO_SECONDARY_DESCRIPTION_RISKYMOD";
            spitDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD" };
            spitDef.mustKeyPress = false;
            spitDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.Croco.FireSpitModded));
            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Croco.FireSpitModded));

            //I hate how this has so many keywords.
            SkillDef biteDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoBite.asset").WaitForCompletion();
            biteDef.cancelSprintingOnActivation = false;
            biteDef.skillDescriptionToken = "CROCO_SECONDARY_ALT_DESCRIPTION_RISKYMOD";
            biteDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_SLAYER", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            biteDef.mustKeyPress = false;

            if (CrocoCore.Cfg.enabled)
            {
                spitDef.baseRechargeInterval = CrocoCore.Cfg.Skills.Neurotoxin.cooldown;
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireSpit", "damageCoefficient", SneedUtils.SneedUtils.FloatToString(CrocoCore.Cfg.Skills.Neurotoxin.damageCoefficient));

                biteDef.baseRechargeInterval = CrocoCore.Cfg.Skills.Bite.cooldown;
            }
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Bite", "damageCoefficient", SneedUtils.SneedUtils.FloatToString(CrocoCore.Cfg.Skills.Bite.damageCoefficient));

            new ModifyM2Spit();
            new ModifyM2Bite();
        }

        //Both Shifts can Blight, otherwise the default ends up being objectively better.
        private void ModifyUtilities(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Leap", "blastDamageCoefficient", "3.2");
            SkillDef utilityDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoLeap.asset").WaitForCompletion();
            SkillDef utilityAltDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoChainableLeap.asset").WaitForCompletion();

            utilityDef.skillDescriptionToken = "CROCO_UTILITY_DESCRIPTION_RISKYMOD";
            utilityDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_STUNNING" };
            utilityAltDef.baseRechargeInterval = CrocoCore.Cfg.Skills.CausticLeap.cooldown;

            utilityAltDef.skillDescriptionToken = "CROCO_UTILITY_ALT1_DESCRIPTION_RISKYMOD";
            utilityAltDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_STUNNING" };
            utilityAltDef.baseMaxStock = 1;
            utilityAltDef.baseRechargeInterval = CrocoCore.Cfg.Skills.FrenziedLeap.cooldown;

            if (CrocoCore.Cfg.enabled)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Leap", "blastDamageCoefficient", SneedUtils.SneedUtils.FloatToString(CrocoCore.Cfg.Skills.CausticLeap.damageCoefficient));

                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.ChainableLeap", "blastDamageCoefficient", SneedUtils.SneedUtils.FloatToString(CrocoCore.Cfg.Skills.FrenziedLeap.damageCoefficient));
            }
            new ModifyShift();
        }

        //Watch out to make sure this isn't making a worse autoplay situation than the original.
        //Seems strong, but isn't something you can solely rely on to clear the map like Vanilla poison.
        private void ModifySpecials(SkillLocator sk)
        {
            SkillDef epidemicDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoDisease.asset").WaitForCompletion();

            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireDiseaseProjectile", "damageCoefficient", "1");
            epidemicDef.skillDescriptionToken = "CROCO_SPECIAL_DESCRIPTION_RISKYMOD";
            epidemicDef.keywordTokens = new string[] { };
            if (CrocoCore.Cfg.enabled)
            {
                epidemicDef.baseRechargeInterval = CrocoCore.Cfg.Skills.Epidemic.cooldown;
            }
            Skills.Epidemic = epidemicDef;
            new ModifySpecial();

            if (SoftDependencies.ScepterPluginLoaded || SoftDependencies.ClassicItemsScepterLoaded)
            {
                BuildScepterSkillDefs(sk);

                if (SoftDependencies.ScepterPluginLoaded) SetupScepter();
                if (SoftDependencies.ClassicItemsScepterLoaded) SetupScepterClassic();
            }
        }

        private void BuildScepterSkillDefs(SkillLocator sk)
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
            diseaseScepterDef.icon = Assets.ScepterSkillIcons.CrocoEpidemicScepter;
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

            Content.Content.entityStates.Add(typeof(FireDiseaseProjectileScepter));
            Content.Content.skillDefs.Add(diseaseScepterDef);

            Skills.EpidemicScepter = diseaseScepterDef;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.EpidemicScepter, "CrocoBody", SkillSlot.Special, 0);
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepterClassic()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.EpidemicScepter, "CrocoBody", SkillSlot.Special, Skills.Epidemic);
        }

        public static bool HasDeeprot(SkillLocator sk)
        {
            bool hasDeeprot = false;
            if (SoftDependencies.SpikestripPlasmaCore && sk)
            {
                hasDeeprot = HasDeeprotInternal(sk);
            }
            return hasDeeprot;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool HasDeeprotInternal(SkillLocator sk)
        {
            bool deeprotEquipped = false;
            if (PlasmaCoreSpikestripContent.Content.Skills.DeepRot.scriptableObject != null)
            {
                foreach(GenericSkill gs in sk.allSkills)
                {
                    if (gs.skillDef == PlasmaCoreSpikestripContent.Content.Skills.DeepRot.scriptableObject.SkillDefinition)
                    {
                        deeprotEquipped = true;
                        break;
                    }
                }
            }
            return deeprotEquipped;
        }
    }

    public static class Skills
    {
        public static SkillDef Epidemic;
        public static SkillDef EpidemicScepter;
    }
}
