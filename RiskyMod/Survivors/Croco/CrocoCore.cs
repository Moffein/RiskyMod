using RoR2;
using R2API;
using UnityEngine;
using RoR2.Skills;

namespace RiskyMod.Survivors.Croco
{
    public class CrocoCore
    {
        //This won't have the option to disable individual features since everything here is too interlinked.
        public static bool enabled = true;

        public static SkillDef M1PoisonDef;
        public static SkillDef M1BlightDef;

        public CrocoCore()
        {
            if (!enabled) return;
            new BlightStack();
            new BiggerMeleeHitbox();

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
            //- Poison Heal Passive: Unique debuffs on an enemy = more heals from melee attacks?
            //- Bring back playstyle based around stacking different DoTs.
            //- Poison on M1: the strongest tank busting tool should require risk to apply.
            //- Blight stacks like bleed, keep it on M2/Shift impact so that it's cooldown-gated.
            //- For M2s, only Bite applies blight? So that players need to stay close at melee range to keep their stacks up.
            //- Make Spit feel more fun to use.
            //- High knockback?
            //- Should it be able to Blight? Thematically it makes sense, but it enables a cowardly playstyle.
            //- Alternative is to make it arc to make it harder to snipe.
            //- Shift Acid Puddle is bigger and deals more damage/proc. Also slows enemies inside it.
            //- R deals 4x150% proccing damage instead of Poison
            //- This might still allow for the AFK playstyle.

            //Debuffs Acrid can use:
            //Poison
            //Blight
            //Epidemic

            //Playtest Notes:
            //Debuff regen devolves into a free regen bonus due to the amount of debuff items. Does not emphasize using Poisons.
                //Regen based on poison in general doesn't really seem to add much to Acrid's gameplay.
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
            new IncreaseRegenerativeHeal();
            ModifyPassives(sk);
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPassives(SkillLocator sk)
        {
            //This nullrefs
            /*for (int i = 0; i < sk.allSkills.Length; i++)
            {
                if (sk.allSkills[i].skillFamily.variants[0].skillDef.skillNameToken == "CROCO_PASSIVE_NAME" && sk.allSkills[i].skillFamily.variants.Length == 2)
                {
                    sk.allSkills[i].skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD" };
                    break;
                }
            }*/
        }
        private void ModifyPrimaries(SkillLocator sk)
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "damageCoefficient", "1.5");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "comboFinisherDamageCoefficient", "4.5");

            sk.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_PRIMARY_DESCRIPTION_RISKYMOD";
            //sk.primary.skillFamily.variants[0].skillDef.canceledFromSprinting = false;    //Removing sprint cancelling breaks this skill super hard.
            sk.primary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_POISON", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            new ModifyM1();

            M1PoisonDef = sk.primary.skillFamily.variants[0].skillDef;
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            //Ranged attack only Weakens
                //Allowing it to Blight allows you to keep up Blight stacks with no risk, overshadowing the Bite.
            //Acrid is overloaded with too many unique debuffs. Consider simplifying.
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireSpit", "damageCoefficient", "2.4");
            sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_SECONDARY_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD" };
            new ModifyM2Spit();

            //I hate how this has so many keywords.
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Bite", "damageCoefficient", "4");
            sk.secondary.skillFamily.variants[1].skillDef.skillDescriptionToken = "CROCO_SECONDARY_ALT_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_SLAYER", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            new ModifyM2Bite();
        }

        //Both Shifts can Blight, otherwise the default ends up being objectively better.
        private void ModifyUtilities(SkillLocator sk)
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Leap", "blastDamageCoefficient", "3.2");
            sk.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_UTILITY_DESCRIPTION_RISKYMOD";
            sk.utility.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_STUNNING"};

            //Might add Regenerative if this is still useless.
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.ChainableLeap", "blastDamageCoefficient", "6.4");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Bite", "damageCoefficient", "4");
            sk.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "CROCO_UTILITY_ALT1_DESCRIPTION_RISKYMOD";
            sk.utility.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_STUNNING"};
            new ModifyShift();
        }

        //Watch out to make sure this isn't making a worse autoplay situation than the original.
        private void ModifySpecials(SkillLocator sk)
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireDiseaseProjectile", "damageCoefficient", "1.2");
            sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_SPECIAL_DESCRIPTION_RISKYMOD";
            sk.special.skillFamily.variants[0].skillDef.keywordTokens = new string[] {};
            new ModifySpecial();
        }
    }
}
