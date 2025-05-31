﻿using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Loader
{
    public class LoaderCore
    {
        public static bool enabled = true;

        public static bool modifyStats = true;

        public static bool zapFistChanges = true;

        public static bool pylonChanges = true;
        public static bool slamChanges = true;

        public static GameObject bodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderBody.prefab").WaitForCompletion();
        private CharacterBody loaderBody;
        public LoaderCore()
        {
            if (!enabled) return;

            loaderBody = bodyPrefab.GetComponent<CharacterBody>();
            SkillLocator sk = bodyPrefab.GetComponent<SkillLocator>();

            ModifyStats(loaderBody);
            ModifySkills(sk);

            //Originally had a big all-around stat nerf, but it pretty much was just nerfing for the sake of nerfing and didn't really expand her gameplay or anything,
            //which is counter to the reason why this mod exists in the first place.
        }

        private void ModifyStats(CharacterBody cb)
        {
            if (!modifyStats) return;
            cb.baseMaxHealth = 140f;
            cb.baseArmor = 0f;
            cb.levelMaxHealth = 42f;
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (zapFistChanges) new ZapConePosition();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (pylonChanges)
            {
                SkillDef pylonDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/ThrowPylon.asset").WaitForCompletion();
                pylonDef.skillDescriptionToken = "LOADER_SPECIAL_DESCRIPTION_RISKYMOD";
                pylonDef.keywordTokens = new string[] { "KEYWORD_MAGNETIC_RISKYMOD" };
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Loader/EntityStates.Loader.ThrowPylon.asset", "damageCoefficient", "0.7");
                new PylonMagnet();
            }

            if (slamChanges)
            {
                new BiggerSlamHitbox();
                new SlamScrapBarrier();
                new SlamDamageType();
            }
        }
    }
}
