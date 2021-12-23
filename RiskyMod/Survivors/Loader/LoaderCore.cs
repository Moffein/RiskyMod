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
