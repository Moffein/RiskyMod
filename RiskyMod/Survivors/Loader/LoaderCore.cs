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

        public static bool zapFistChanges = true;

        public static bool pylonChanges = true;
        public static bool slamChanges = true;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody");
        private CharacterBody loaderBody;
        public LoaderCore()
        {
            if (!enabled) return;

            loaderBody = bodyPrefab.GetComponent<CharacterBody>();
            SkillLocator sk = bodyPrefab.GetComponent<SkillLocator>();
            SprintQoL(sk);

            new FixScepterUtilityBarrier();
            ModifyStats(loaderBody);
            ModifySkills(sk);

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

        private void ModifyStats(CharacterBody cb)
        {
            if (!modifyStats) return;
            cb.baseMaxHealth = 140f;
            cb.baseArmor = 12f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (zapFistChanges)
            {
                new ZapConePosition();
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (pylonChanges)
            {
                sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "LOADER_SPECIAL_DESCRIPTION_RISKYMOD";
                sk.special.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_MAGNETIC_RISKYMOD" };
                sk.special.skillFamily.variants[0].skillDef.cancelSprintingOnActivation = false;
                SneedUtils.SneedUtils.SetEntityStateField("entitystates.loader.throwpylon", "damageCoefficient", "0.7");
                new PylonMagnet();
            }

            if (slamChanges)
            {
                sk.special.skillFamily.variants[1].skillDef.cancelSprintingOnActivation = false;
                new BiggerSlamHitbox();
                new SlamScrapBarrier();
                new SlamDamageType();
            }
        }
    }
}
