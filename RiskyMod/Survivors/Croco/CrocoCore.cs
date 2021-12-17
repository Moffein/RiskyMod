using RoR2;
using R2API;
using UnityEngine;

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
        }

        private void ModifyStats(CharacterBody cb)
        {
            cb.baseDamage = 12f;
            cb.levelDamage = cb.baseDamage * 0.2f;
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPassives(sk);
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
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
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "damageCoefficient", "2.5");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Slash", "comboFinisherDamageCoefficient", "5");

            sk.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_PRIMARY_DESCRIPTION_RISKYMOD";
            sk.primary.skillFamily.variants[0].skillDef.canceledFromSprinting = false;
            sk.primary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_POISON", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            new ModifyM1();
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireSpit", "damageCoefficient", "3");
            sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CROCO_SECONDARY_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[0].skillDef.baseRechargeInterval = 2f;
            sk.secondary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD" };
            new ModifyM2Spit();
            //Need to figure out some other effect besides Blight for Spit.
            //Goo that slows/roots enemies?

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.Bite", "damageCoefficient", "4");
            sk.secondary.skillFamily.variants[1].skillDef.skillDescriptionToken = "CROCO_SECONDARY_ALT_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_BLIGHT_RISKYMOD", "KEYWORD_SLAYER", "KEYWORD_RAPID_REGEN_RISKYMOD" };
            new ModifyM2Bite();
        }
    }
}
