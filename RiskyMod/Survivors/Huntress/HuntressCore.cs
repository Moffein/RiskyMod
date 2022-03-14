using RoR2;
using UnityEngine;
using R2API;
using MonoMod.Cil;
using System;
using EntityStates;
using Mono.Cecil.Cil;
using System.Runtime.CompilerServices;
using EntityStates.RiskyMod.Huntress;
using RoR2.Skills;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Huntress
{
    public class HuntressCore
    {
        public static bool enabled = true;

        public static bool strafeChanges = true;
        public static bool flurryChanges = true;

        public static bool laserGlaiveChanges = true;

        public static bool blinkChanges = true;

        public static bool arrowRainChanges = true;
        public static bool ballistaChanges = true;

        public static bool increaseAngle = true;
        public static BullseyeSearch.SortMode HuntressTargetingMode = BullseyeSearch.SortMode.Angle;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody");

        public HuntressCore()
        {
            if (!enabled) return;
            TrackingChanges();
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void TrackingChanges()
        {
            On.RoR2.HuntressTracker.Start += (orig, self) =>
            {
                orig(self);
                if (increaseAngle)
                {
                    self.maxTrackingAngle = 45f;
                }
            };
            IL.RoR2.HuntressTracker.SearchForTarget += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchStfld(typeof(BullseyeSearch), "sortMode")
                    );
                c.EmitDelegate<Func<BullseyeSearch.SortMode, BullseyeSearch.SortMode>>(orig =>
                {
                    return HuntressTargetingMode;
                });
            };
        }


        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (strafeChanges)
            {
                sk.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "HUNTRESS_PRIMARY_DESCRIPTION_RISKYMOD";
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.FireSeekingArrow", "orbDamageCoefficient", "2");
            }

            if (flurryChanges)
            {
                sk.primary.skillFamily.variants[1].skillDef.skillDescriptionToken = "HUNTRESS_PRIMARY_ALT_DESCRIPTION_RISKYMOD";
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow", "orbDamageCoefficient", "1.2");
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow", "orbProcCoefficient", "1");
            }


            //This fixes Flurry losing arrows at high attack speed.
            On.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.FireOrbArrow += (orig, self) =>
            {
                if (!NetworkServer.active)
                {
                    self.firedArrowCount++;
                }
                orig(self);
            };

            IL.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCall<EntityState>("get_isAuthority")
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);    //the entitystate
                c.EmitDelegate<Func<bool, EntityStates.Huntress.HuntressWeapon.FireSeekingArrow, bool>>((flag, self) =>
                {
                    return flag && self.firedArrowCount >= self.maxArrowCount;
                });
            };
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (!laserGlaiveChanges) return;
            sk.secondary.skillFamily.variants[0].skillDef.baseRechargeInterval = 6f;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.ThrowGlaive", "baseDuration", "0.8");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.ThrowGlaive", "glaiveProcCoefficient", "1");
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (blinkChanges)
            {
                sk.utility.skillFamily.variants[0].skillDef.baseRechargeInterval = 6f;
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (arrowRainChanges)
            {
                sk.special.skillFamily.variants[0].skillDef.baseRechargeInterval = 10f;
                sk.special.skillFamily.variants[0].skillDef.beginSkillCooldownOnSkillEnd = true;
                sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "HUNTRESS_SPECIAL_DESCRIPTION_RISKYMOD";
                new ArrowRainBuff();
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.ArrowRain", "damageCoefficient", "4.2");   //ArrowRainBuff adjusts this value to be accurate
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.ArrowRain", "projectilePrefab", ArrowRainBuff.arrowRainObject);

                if (RiskyMod.ScepterPluginLoaded)
                {
                    SetupScepter(sk);
                }
            }

            if (ballistaChanges)
            {
                sk.special.skillFamily.variants[1].skillDef.baseRechargeInterval = 10f;
                sk.special.skillFamily.variants[1].skillDef.beginSkillCooldownOnSkillEnd = true;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter(SkillLocator sk)
        {
            ArrowRainBuff.ScepterProjectileSetup();

            Content.Content.entityStates.Add(typeof(BeginArrowRainScepter));
            Content.Content.entityStates.Add(typeof(ArrowRainScepter));

            ArrowRainScepter.muzzleFlashEffect = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Huntress.ArrowRain", "muzzleFlashEffect");
            ArrowRainScepter.areaIndicatorPrefab = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Huntress.ArrowRain", "areaIndicatorPrefab");

            SkillDef arrowRainDef = SkillDef.CreateInstance<SkillDef>();
            arrowRainDef.activationState = new SerializableEntityStateType(typeof(BeginArrowRainScepter));
            arrowRainDef.activationStateMachineName = sk.special.skillFamily.variants[0].skillDef.activationStateMachineName;
            arrowRainDef.baseMaxStock = 1;
            arrowRainDef.baseRechargeInterval = 10f;
            arrowRainDef.beginSkillCooldownOnSkillEnd = sk.special.skillFamily.variants[0].skillDef.beginSkillCooldownOnSkillEnd;
            arrowRainDef.canceledFromSprinting = sk.special.skillFamily.variants[0].skillDef.canceledFromSprinting;
            arrowRainDef.dontAllowPastMaxStocks = true;
            arrowRainDef.forceSprintDuringState = false;
            arrowRainDef.fullRestockOnAssign = true;
            arrowRainDef.icon = AncientScepter.Assets.SpriteAssets.HuntressRain2;
            arrowRainDef.interruptPriority = sk.special.skillFamily.variants[0].skillDef.interruptPriority;
            arrowRainDef.isCombatSkill = true;
            arrowRainDef.keywordTokens = new string[] {};
            arrowRainDef.mustKeyPress = false;
            arrowRainDef.cancelSprintingOnActivation = true;
            arrowRainDef.rechargeStock = 1;
            arrowRainDef.requiredStock = 1;
            arrowRainDef.skillName = "ArrowRainScepter";
            arrowRainDef.skillNameToken = "ANCIENTSCEPTER_HUNTRESS_RAINNAME";
            arrowRainDef.skillDescriptionToken = "HUNTRESS_SPECIAL_SCEPTER_DESCRIPTION_RISKYMOD";
            arrowRainDef.stockToConsume = 1;
            Content.Content.skillDefs.Add(arrowRainDef);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(arrowRainDef, "HuntressBody", SkillSlot.Special, 0);
        }
    }
}
