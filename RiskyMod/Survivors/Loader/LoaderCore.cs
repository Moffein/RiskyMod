using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Loader
{
    public class LoaderCore
    {
        public static bool enabled = true;
        private CharacterBody loaderBody;
        public LoaderCore()
        {
            if (!enabled) return;
            loaderBody = RoR2Content.Survivors.Loader.bodyPrefab.GetComponent<CharacterBody>();
            ModifyStats(loaderBody);
            ModifySkills(RoR2Content.Survivors.Loader.bodyPrefab.GetComponent<SkillLocator>());

            //Loader lacks proper fail conditions
                //At close range, she generates barrier and can bail out any time with M2/Shift
                //High natural tankiness even if she does get hit through all of that
                //Only thing that can stop her is accidentally ramming into a Malachite spike while using her Shift.
            //Mechanically, M2+Shift feel fine.
                //Shift cooldown might be too low.
                //Shift damage should be lower, and more affected by Grappling.
            //M2+Shift give her too much mobility.
                //Increasing M2 cooldown would feel terrible.
                //Increasing Shift cooldown would be *balanced*, but she doesn't have anything to do when her Shift is offline.
            //M1 lets her tank in crowds, but doesn't really mesh-in with her kit.
                //Replacing M1 with Visions doesn't make you lose anything important.
            //Default R is boring, but needed to deal with wisps.
                //How can it be reworked?
            //Alt R is fine(?), scaling damage/AOE based on fall distance would be fun.
                //But need to balance the rest of her kit first, if it's even possible.
        }

        private void ModifyStats(CharacterBody cb)
        {
            cb.baseMaxHealth = 140f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;

            cb.baseArmor = 20f;
        }

        private void ModifySkills(SkillLocator sk)
        {
            //ModifyPassives(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
        }
        private void ModifyPassives(SkillLocator sk)
        {
            sk.passiveSkill.skillDescriptionToken = "LOADER_PASSIVE_DESCRIPTION_RISKYMOD";
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            sk.secondary.skillFamily.variants[0].skillDef.cancelSprintingOnActivation = false;
            sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "LOADER_SECONDARY_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };

            sk.secondary.skillFamily.variants[1].skillDef.cancelSprintingOnActivation = false;
            new DefaultGrappleStun();
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            sk.utility.skillFamily.variants[0].skillDef.cancelSprintingOnActivation = false;
            sk.utility.skillFamily.variants[1].skillDef.cancelSprintingOnActivation = false;
        }
    }
}
