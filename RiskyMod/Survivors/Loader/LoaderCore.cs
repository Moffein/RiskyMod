using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Loader
{
    public class LoaderCore
    {
        public static bool enabled = true;

        public static bool shiftCancelsSprint = false;
        public static bool grappleCancelsSprint = false;

        public static bool modifyStats = true;
        public static bool modifyPrimaries = true;
        public static bool modifySecondaries = true;
        public static bool modifyUtilities = true;
        public static bool modifySpecials = true;

        private CharacterBody loaderBody;
        public LoaderCore()
        {
            if (!enabled) return;

            loaderBody = RoR2Content.Survivors.Loader.bodyPrefab.GetComponent<CharacterBody>();
            SkillLocator sk = RoR2Content.Survivors.Loader.bodyPrefab.GetComponent<SkillLocator>();
            SprintQoL(sk);

            new BiggerSlamHitbox();
            ModifySkills(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);

            //Originally had a big all-around stat nerf, but it pretty much was just nerfing for the sake of nerfing and didn't really expand her gameplay or anything,
            //which is counter to the reason why this mod exists in the first place.
        }

        private void SprintQoL(SkillLocator sk)
        {
            sk.secondary.skillFamily.variants[0].skillDef.cancelSprintingOnActivation = grappleCancelsSprint;
            sk.secondary.skillFamily.variants[1].skillDef.cancelSprintingOnActivation = grappleCancelsSprint;

            sk.utility.skillFamily.variants[0].skillDef.cancelSprintingOnActivation = shiftCancelsSprint;
            sk.utility.skillFamily.variants[1].skillDef.cancelSprintingOnActivation = shiftCancelsSprint;
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (!modifyUtilities) return;
            new ZapConePosition();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (!modifySpecials) return;
            new SlamScrapBarrier();
            new BiggerSlamHitbox();
            new SlamDamageType();
        }
    }
}
