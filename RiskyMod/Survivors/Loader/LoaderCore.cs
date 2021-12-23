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
            //Note: Grappling well gives around a 2x bonus to Shift damage
            //M2+Shift give her too much mobility.
                //Increasing M2 cooldown would feel terrible.
                //Increasing Shift cooldown is *balanced*, but she doesn't have anything to do when her Shift is offline.
            //M1 lets her tank, but doesn't really mesh-in with her kit.
                //Replacing M1 with Visions doesn't make you lose anything important.
            //Default R is boring, but needed to deal with wisps.
                //How can it be reworked?
            //Alt R essentially just gives you an extra charge of Shift at the cost of being able to deal with Wisps.
                //Damage/AOE based on fall height would be fun.
                //Objectively better than Default R if you have teammates to shoot the wisps, Pylon needs to offer more than just plain AOE damage.
        }

        private void ModifyStats(CharacterBody cb)
        {
            cb.baseMaxHealth = 120f;
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
            sk.utility.skillFamily.variants[0].skillDef.baseRechargeInterval = 7f;

            sk.utility.skillFamily.variants[1].skillDef.cancelSprintingOnActivation = false;
            sk.utility.skillFamily.variants[1].skillDef.baseRechargeInterval = 7f;
        }
    }
}
