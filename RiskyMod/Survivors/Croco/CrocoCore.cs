using RoR2;
using R2API;
using UnityEngine;
using RoR2.Skills;
using System.Runtime.CompilerServices;
using EntityStates.RiskyMod.Croco;
using EntityStates;

namespace RiskyMod.Survivors.Croco
{
    public class CrocoCore
    {
        //This won't have the option to disable individual features since everything here is too interlinked.
        public static bool enabled = true;

        public CrocoCore()
        {
            if (!enabled) return;
            new BlightStack();
            new BiggerMeleeHitbox();
            new ExtendSpawnInvuln();
            new RemovePoisonDamageCap();

            ModifyStats(RoR2Content.Survivors.Croco.bodyPrefab.GetComponent<CharacterBody>());
            ModifySkills(RoR2Content.Survivors.Croco.bodyPrefab.GetComponent<SkillLocator>());

            //Things to Address:
            //- Actual damage output is already good with both passives.
            //- Poison allows for players to AFK with R and clear the stage regardless of time scaling.
            //- Blight falls off hard past the first loop.
            //- Melee survivability is lacking, Regenerative is mediocre.
            //- No incentive to actually melee, Bite is nothing but a novelty.
            //- Visions of Heresy is objectively better than melee.
            //- Melee playstyle (and playstyle in general) seems to lack depth. Just spamming all abilities when they're off cooldown.

            //Solutions?
            //- Poison Heal Passive: Unique debuffs on an enemy = more heals from melee attacks? [Scrapped]
                //Increased healing ends up making him too much like Loader: stand in front of enemies and hold M1.
            //- Bring back playstyle based around stacking different DoTs.
            //- Poison on M1: the strongest tank busting tool should require risk to apply.
            //- Blight stacks like bleed, keep it on M2/Shift impact so that it's cooldown-gated.
            //- For M2s, only Bite applies blight? So that players need to stay close at melee range to keep their stacks up.
                //- Spit seems to need to apply Blight or else the kit ends up feeling weird.
            //- Make Spit feel more fun to use.
                //- High knockback?
                //- Should it be able to Blight? Thematically it makes sense, but it enables a cowardly playstyle.
            //- Alternative is to make it arc to make it harder to snipe.
            //- Shift Acid Puddle is bigger and deals more damage/proc. Also slows enemies inside it.
            //- R deals 6x100% proccing damage instead of Poison
            //- This might still allow for the AFK playstyle.

            //Debuffs Acrid can use:
            //Poison
            //Blight
            //Epidemic

            //Playtest Notes:
            //Debuff regen devolves into a free regen bonus due to the amount of debuff items. Does not emphasize using Poisons.
                //Regen based on poison in general doesn't really seem to add much to Acrid's gameplay since he always is poisoning things.
            //Weaken on Spit M2 feels wrong due to being able to insta trigger Death Mark with his base loadout. Too many unique debuffs in the kit.
            //Acrid's damage feels too high, he just melts things without a need to rely on kiting/DoT.
                //Lower his the damage of his proccing skills, force him to rely on Blight Stacking to kill solo targets.
                //Crowd damage should remain his specialty.
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
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "comboFinisherDamageCoefficient", "5");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "baseDuration", "1.2");

            sk.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_PRIMARY_DESCRIPTION_RISKYMOD";
            sk.primary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_POISON_RISKYMOD", "KEYWORD_RAPID_REGEN" };
            new ModifyM1();
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_SECONDARY_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD" };
            sk.secondary.skillFamily.variants[0].skillDef.baseRechargeInterval = 3f;
            new ModifyM2Spit();

            //I hate how this has so many keywords.
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Bite", "damageCoefficient", "4");
            sk.secondary.skillFamily.variants[1].skillDef.skillDescriptionToken = "CROCO_SECONDARY_ALT_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_SLAYER", "KEYWORD_RAPID_REGEN" };
            new ModifyM2Bite();
        }

        //Both Shifts can Blight, otherwise the default ends up being objectively better.
        private void ModifyUtilities(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Leap", "blastDamageCoefficient", "3.2");
            sk.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_UTILITY_DESCRIPTION_RISKYMOD";
            sk.utility.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_STUNNING"};

            //Might add Regenerative if this is still useless.
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
