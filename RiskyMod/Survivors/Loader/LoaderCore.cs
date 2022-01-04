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

            ModifyStats(loaderBody);
            ModifySkills(sk);

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
            cb.baseRegen = 1f;
            cb.baseArmor = 12f;

            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.levelRegen = cb.baseRegen * 0.2f;
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (!modifySecondaries) return;
            sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "LOADER_SECONDARY_DESCRIPTION_RISKYMOD";
            sk.secondary.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            new DefaultGrappleStun();
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (!modifyUtilities) return;
            sk.utility.skillFamily.variants[0].skillDef.baseRechargeInterval = 5f;
            sk.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "LOADER_UTILITY_DESCRIPTION_RISKYMOD";

            sk.utility.skillFamily.variants[1].skillDef.baseRechargeInterval = 5f;
            sk.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "LOADER_UTILITY_ALT1_DESCRIPTION_RISKYMOD";

            SneedUtils.SneedUtils.SetEntityStateField("entitystates.loader.baseswingchargedfist", "velocityDamageCoefficient", "0.2");  //orig 0.3
            //new UtilityDamageType();
            new VelocityScaling();
            new ModifyZapDamage();

            //SneedUtils.SneedUtils.DumpEntityStateConfig("entitystates.loader.baseswingchargedfist");
            //SneedUtils.SneedUtils.DumpEntityStateConfig("entitystates.loader.swingchargedfist");
            //SneedUtils.SneedUtils.DumpEntityStateConfig("entitystates.loader.swingzapfist");
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (!modifySpecials) return;
            sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "LOADER_SPECIAL_DESCRIPTION_RISKYMOD";
            sk.special.skillFamily.variants[0].skillDef.keywordTokens = new string[] { "KEYWORD_MAGNETIC_RISKYMOD" };
            sk.special.skillFamily.variants[0].skillDef.cancelSprintingOnActivation = false;
            SneedUtils.SneedUtils.SetEntityStateField("entitystates.loader.throwpylon", "damageCoefficient", "0.5");
            new PylonMagnet();

            sk.special.skillFamily.variants[1].skillDef.skillDescriptionToken = "LOADER_SPECIAL_ALT_DESCRIPTION_RISKYMOD";
            sk.special.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_HEAVY" };
            sk.special.skillFamily.variants[1].skillDef.cancelSprintingOnActivation = false;

            LoadoutAPI.AddSkill(typeof(EntityStates.RiskyMod.Loader.PreGroundSlamScaled));
            LoadoutAPI.AddSkill(typeof(EntityStates.RiskyMod.Loader.GroundSlamScaled));
            EntityStates.RiskyMod.Loader.GroundSlamScaled.fistEffectPrefab = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Loader.GroundSlam", "fistEffectPrefab");
            sk.special.skillFamily.variants[1].skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.Loader.PreGroundSlamScaled));
            new SlamScrapBarrier();
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Loader.GroundSlam");
        }
    }
}
