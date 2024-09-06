using RoR2;
using UnityEngine;
using System;
using EntityStates;
using R2API;
using System.Runtime.CompilerServices;
using RoR2.Skills;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using RiskyMod.Survivors.Mage.Components;
using EntityStates.RiskyMod.Mage.Weapon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using BepInEx.Configuration;
using RiskyMod.Survivors.Mage.SkillTweaks;

namespace RiskyMod.Survivors.Mage
{
    public class MageCore
    {
        public static bool enabled = true;

        public static bool modifyFireBolt = true;
        public static bool modifyPlasmaBolt = true;

        public static bool m2RemoveNanobombGravity = true;

        public static ConfigEntry<bool> flamethrowerSprintCancel;
        public static bool flamethrowerRangeExtend = true;

        public static bool ionSurgeShock = true;

        public static bool ionSurgeUtility = true;
        public static bool ionSurgeUtilityKeepSpecial = false;

        public static bool enableFireUtility = true;
        public static bool enableLightningSpecial = true;

        public static bool iceWallRework = true;

        public static int specialLightningVariantIndex; //DestroyedClone Scepter needs variant index

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MageBody");

        public static ConfigEntry<bool> utilitySelfKnockback;

        public MageCore()
        {
            if (!enabled) return;
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }
        private void ModifySkills(SkillLocator sk)
        {
            HandleIonSurge(sk);

            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            SkillDef fireBoltDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFireFirebolt.asset").WaitForCompletion();
            SkillDef plasmaBoltDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFireLightningBolt.asset").WaitForCompletion();


            //TODO: Did I accidentally delete the thing that changes damage?
            if (modifyFireBolt)
            {
                fireBoltDef.skillDescriptionToken = "MAGE_PRIMARY_FIRE_DESCRIPTION_RISKYMOD";
            }

            if (modifyPlasmaBolt)
            {
                plasmaBoltDef.skillDescriptionToken = "MAGE_PRIMARY_LIGHTNING_DESCRIPTION_RISKYMOD";
            }
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (m2RemoveNanobombGravity)
            {
                GameObject projectile = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageLightningBombProjectile");
                AntiGravityForce agf = projectile.GetComponent<AntiGravityForce>();
                agf.antiGravityCoefficient = 1f;
            }
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (iceWallRework)
            {
                //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Mage.Weapon.PrepWall");
                Content.Content.entityStates.Add(typeof(PrepIceWall));

                SkillDef iceWallDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyWall.asset").WaitForCompletion();
                iceWallDef.skillDescriptionToken = "MAGE_UTILITY_ICE_DESCRIPTION_RISKYMOD";
                iceWallDef.canceledFromSprinting = !SoftDependencies.RtAutoSprintLoaded;
                iceWallDef.activationState = new SerializableEntityStateType(typeof(PrepIceWall));

                Skills.PrepIceWall = iceWallDef;
            }

            new IceWallDefense();

            if (enableFireUtility)
            {
                //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Mage.FlyUpState"); SkillDef ionSurge2 = ScriptableObject.CreateInstance<SkillDef>();
                Content.Content.entityStates.Add(typeof(PrepFireStorm));

                SkillDef fireStorm = ScriptableObject.CreateInstance<SkillDef>();
                fireStorm.activationState = new SerializableEntityStateType(typeof(PrepFireStorm));
                fireStorm.activationStateMachineName = "Weapon";
                fireStorm.baseMaxStock = 1;
                fireStorm.baseRechargeInterval = 12f;
                fireStorm.beginSkillCooldownOnSkillEnd = true;
                fireStorm.canceledFromSprinting = !SoftDependencies.RtAutoSprintLoaded;
                fireStorm.cancelSprintingOnActivation = true;
                fireStorm.dontAllowPastMaxStocks = true;
                fireStorm.forceSprintDuringState = false;
                fireStorm.fullRestockOnAssign = true;
                fireStorm.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlamethrower.asset").WaitForCompletion().icon;
                fireStorm.interruptPriority = InterruptPriority.PrioritySkill;
                fireStorm.isCombatSkill = true;
                fireStorm.keywordTokens = new string[] { "KEYWORD_IGNITE" };
                fireStorm.mustKeyPress = false;
                fireStorm.rechargeStock = 1;
                fireStorm.requiredStock = 1;
                fireStorm.resetCooldownTimerOnUse = false;
                fireStorm.skillDescriptionToken = "MAGE_UTILITY_FIRE_DESCRIPTION_RISKYMOD";
                fireStorm.skillNameToken = "MAGE_UTILITY_FIRE_NAME_RISKYMOD";
                fireStorm.skillName = "RiskyModFireStorm";
                fireStorm.stockToConsume = 1;
                SneedUtils.SneedUtils.FixSkillName(fireStorm);

                Skills.PrepFireStorm = fireStorm;
                Content.Content.skillDefs.Add(Skills.PrepFireStorm);

                SkillFamily utilitySkillFamily = sk.utility.skillFamily;
                Array.Resize(ref utilitySkillFamily.variants, utilitySkillFamily.variants.Length + 1);
                utilitySkillFamily.variants[utilitySkillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = fireStorm,
                    viewableNode = new ViewablesCatalog.Node(fireStorm.skillNameToken, false)
                };

                GameObject fireStormProjectile = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireTornado").InstantiateClone("RiskyMod_MageFireStorm", true);

                ProjectileSimple ps = fireStormProjectile.GetComponent<ProjectileSimple>();
                //ps.lifetime = 6f; //Particle doesn't scale to lifetime. Use FireStormExtender as a hacky solution to this.

                ProjectileOverlapAttack poa = fireStormProjectile.GetComponent<ProjectileOverlapAttack>();
                poa.overlapProcCoefficient = 1f;
                poa.resetInterval = 0.4f;  //0.3 vanilla

                poa.damageCoefficient = (1f / Mathf.Floor(ps.lifetime / poa.resetInterval)) * 0.5f;  //Div by 2 because 2 projectiles spawn, so damage should be distributed between them.

                ProjectileDamage pd = fireStormProjectile.GetComponent<ProjectileDamage>();
                pd.damageType = DamageType.IgniteOnHit;

                Content.Content.projectilePrefabs.Add(fireStormProjectile);
                EntityStates.RiskyMod.Mage.Weapon.PrepFireStorm.projectilePrefab = fireStormProjectile;

                GameObject fireStormProjectile2 = fireStormProjectile.InstantiateClone("RiskyMod_MageFireStorm2", true);
                Content.Content.projectilePrefabs.Add(fireStormProjectile2);
                FireStormExtender.projectilePrefab = fireStormProjectile2;
                fireStormProjectile.AddComponent<FireStormExtender>();  //HACKY
            }

            //Update skill descriptions based on push setting
            UpdatePushSetting(null, null);
        }

        private void ModifySpecials(SkillLocator sk)
        {
            flamethrowerSprintCancel.SettingChanged += ValidateFlamethrowerSprintSettings;

           //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Mage.Weapon.Flamethrower");
           SkillDef flamethrowerDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlamethrower.asset").WaitForCompletion();
            Skills.SpecialFlamethrowerVanilla = flamethrowerDef;
            flamethrowerDef.canceledFromSprinting = flamethrowerSprintCancel.Value;
            if (flamethrowerRangeExtend)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.Flamethrower", "maxDistance", "25");  //20 vanilla
            }

            if (enableLightningSpecial)
            {
                EntityStates.RiskyMod.Mage.SpecialLightning.laserEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageLightningLaser.prefab").WaitForCompletion().InstantiateClone("RiskyMod_LaserBoltTracer", false);
                DestroyOnTimer dt = EntityStates.RiskyMod.Mage.SpecialLightning.laserEffectPrefab.AddComponent<DestroyOnTimer>();
                dt.duration = 0.5f;
                Content.Content.effectDefs.Add(new EffectDef(EntityStates.RiskyMod.Mage.SpecialLightning.laserEffectPrefab));

                Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Mage.SpecialLightning));
                SkillDef skillDef = SkillDef.CreateInstance<SkillDef>();
                skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.Mage.SpecialLightning));
                skillDef.activationStateMachineName = "Weapon";
                skillDef.baseMaxStock = 1;
                skillDef.baseRechargeInterval = 5f;
                skillDef.beginSkillCooldownOnSkillEnd = true;
                skillDef.canceledFromSprinting = flamethrowerSprintCancel.Value && !SoftDependencies.RtAutoSprintLoaded;
                skillDef.dontAllowPastMaxStocks = true;
                skillDef.forceSprintDuringState = false;
                skillDef.fullRestockOnAssign = true;
                skillDef.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlyUp.asset").WaitForCompletion().icon;
                skillDef.interruptPriority = InterruptPriority.Skill;
                skillDef.isCombatSkill = true;
                skillDef.keywordTokens = new string[] { };
                skillDef.mustKeyPress = true;
                skillDef.cancelSprintingOnActivation = true;
                skillDef.rechargeStock = 1;
                skillDef.requiredStock = 1;
                skillDef.skillName = "RiskymodMageSpecialSithLightning";
                skillDef.skillNameToken = "MAGE_SPECIAL_SITHLIGHTNING_NAME_RISKYMOD";
                skillDef.skillDescriptionToken = "MAGE_SPECIAL_SITHLIGHTNING_DESCRIPTION_RISKYMOD";
                skillDef.stockToConsume = 1;
                SneedUtils.SneedUtils.FixSkillName(skillDef);
                Content.Content.skillDefs.Add(skillDef);

                EntityStates.RiskyMod.Mage.SpecialLightning.gauntletMissEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightning.prefab").WaitForCompletion().InstantiateClone("RiskyMod_MageSpecialLightningMiss", false);
                EffectComponent ec = EntityStates.RiskyMod.Mage.SpecialLightning.gauntletMissEffectPrefab.GetComponent<EffectComponent>();
                ec.soundName = "Play_item_proc_chain_lightning";
                Content.Content.effectDefs.Add(new EffectDef(EntityStates.RiskyMod.Mage.SpecialLightning.gauntletMissEffectPrefab));

                EntityStates.RiskyMod.Mage.SpecialLightning.gauntletEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightningLargeWithTrail.prefab").WaitForCompletion().InstantiateClone("RiskyMod_MageSpecialLightning", false);
                ec = EntityStates.RiskyMod.Mage.SpecialLightning.gauntletEffectPrefab.GetComponent<EffectComponent>();
                ec.soundName = "Play_mage_m1_cast_lightning";//"Play_RiskyMod_Mage_Special_Lightning2";//"Play_RiskyMod_RoR1Lightning"; //other SFX too annoying
                Content.Content.effectDefs.Add(new EffectDef(EntityStates.RiskyMod.Mage.SpecialLightning.gauntletEffectPrefab));

                Skills.SpecialLightning = skillDef;
                Array.Resize(ref sk.special.skillFamily.variants, sk.special.skillFamily.variants.Length + 1);
                sk.special.skillFamily.variants[sk.special.skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = Skills.SpecialLightning,
                    unlockableDef = null,
                    viewableNode = new ViewablesCatalog.Node(Skills.SpecialLightning.skillName, false, null)
                };
                specialLightningVariantIndex = sk.special.skillFamily.variants.Length - 1;
            }

            if (SoftDependencies.ScepterPluginLoaded || SoftDependencies.ClassicItemsScepterLoaded)
            {
                BuildScepterSkillDefs(sk);
                if (SoftDependencies.ScepterPluginLoaded) SetupScepter();
                if (SoftDependencies.ClassicItemsScepterLoaded) SetupScepterClassic();
            }
        }

        private void ValidateFlamethrowerSprintSettings(object sender, EventArgs e)
        {
            if (Skills.SpecialFlamethrowerVanilla)
            {
                Skills.SpecialFlamethrowerVanilla.canceledFromSprinting = flamethrowerSprintCancel.Value;
            }
            if (Skills.SpecialLightning)
            {
                Skills.SpecialLightning.canceledFromSprinting = !SoftDependencies.RtAutoSprintLoaded && flamethrowerSprintCancel.Value;
            }
            if (Skills.SpecialLightningScepter)
            {
                Skills.SpecialLightningScepter.canceledFromSprinting = !SoftDependencies.RtAutoSprintLoaded && flamethrowerSprintCancel.Value;
            }
        }

        private void BuildScepterSkillDefs(SkillLocator sk)
        {
            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Mage.SpecialLightningScepter));
            SkillDef skillDef = SkillDef.CreateInstance<SkillDef>();
            skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.Mage.SpecialLightningScepter));
            skillDef.activationStateMachineName = "Weapon";
            skillDef.baseMaxStock = 1;
            skillDef.baseRechargeInterval = 5f;
            skillDef.beginSkillCooldownOnSkillEnd = true;
            skillDef.canceledFromSprinting = flamethrowerSprintCancel.Value;
            skillDef.dontAllowPastMaxStocks = true;
            skillDef.forceSprintDuringState = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlyUp.asset").WaitForCompletion().icon;
            skillDef.interruptPriority = InterruptPriority.Skill;
            skillDef.isCombatSkill = true;
            skillDef.keywordTokens = new string[] { };
            skillDef.mustKeyPress = true;
            skillDef.cancelSprintingOnActivation = true;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.skillName = "RiskymodMageSpecialSithLightningScepter";
            skillDef.skillNameToken = "MAGE_SPECIAL_SITHLIGHTNING_SCEPTER_NAME_RISKYMOD";
            skillDef.skillDescriptionToken = "MAGE_SPECIAL_SITHLIGHTNING_SCEPTER_DESCRIPTION_RISKYMOD";
            skillDef.stockToConsume = 1;
            SneedUtils.SneedUtils.FixSkillName(skillDef);
            Content.Content.skillDefs.Add(skillDef);
            Skills.SpecialLightningScepter = skillDef;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            if (MageCore.enableLightningSpecial) AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.SpecialLightningScepter, "MageBody", Skills.SpecialLightning);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepterClassic()
        {
            if (MageCore.enableLightningSpecial) ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.SpecialLightningScepter, "MageBody", SkillSlot.Special, Skills.SpecialLightning);
        }

        private void HandleIonSurge(SkillLocator sk)
        {
            SkillDef surgeDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlyUp.asset").WaitForCompletion();
            if (ionSurgeShock)
            {
                surgeDef.skillDescriptionToken = "MAGE_SPECIAL_LIGHTNING_DESCRIPTION_RISKYMOD";
                string keyword = "KEYWORD_SHOCKING";
                surgeDef.keywordTokens = new string[] { keyword };

                IL.EntityStates.Mage.FlyUpState.OnEnter += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if(c.TryGotoNext(
                         x => x.MatchCallvirt<BlastAttack>("Fire")
                        ))
                    {
                        c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                        {
                            blastAttack.damageType = DamageType.Shock5s;
                            return blastAttack;
                        });
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("RiskyMod: Mage HandleIonSurge IL Hook failed");
                    }
                };

            }

            if (ionSurgeUtility)
            {
                surgeDef.forceSprintDuringState = false;
                //Remove Ion Surge from Specials
                if (!ionSurgeUtilityKeepSpecial) sk.special.skillFamily.variants = sk.special.skillFamily.variants.Where(v => v.skillDef.activationState.stateType != typeof(EntityStates.Mage.FlyUpState)).ToArray();

                //Add to Utility
                SkillFamily utilitySkillFamily = sk.utility.skillFamily;
                Array.Resize(ref utilitySkillFamily.variants, utilitySkillFamily.variants.Length + 1);
                utilitySkillFamily.variants[utilitySkillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = surgeDef,
                    unlockableDef = Addressables.LoadAssetAsync<UnlockableDef>("RoR2/Base/Mage/Skills.Mage.FlyUp.asset").WaitForCompletion(),
                    viewableNode = new ViewablesCatalog.Node(surgeDef.skillNameToken, false)
                };
            }
        }

        public static void UpdatePushSetting(object sender, System.EventArgs e)
        {
            if (utilitySelfKnockback.Value)
            {
                if (Skills.PrepFireStorm) Skills.PrepFireStorm.skillDescriptionToken = "MAGE_UTILITY_FIRE_DESCRIPTION_RISKYMOD";
                if (iceWallRework)
                {
                    if (Skills.PrepIceWall) Skills.PrepIceWall.skillDescriptionToken = "MAGE_UTILITY_ICE_DESCRIPTION_RISKYMOD";
                }
            }
            else
            {
                if (Skills.PrepFireStorm) Skills.PrepFireStorm.skillDescriptionToken = "MAGE_UTILITY_FIRE_DESCRIPTION_NOPUSH_RISKYMOD";
                if (iceWallRework)
                {
                    if (Skills.PrepIceWall) Skills.PrepIceWall.skillDescriptionToken = "MAGE_UTILITY_ICE_DESCRIPTION_NOPUSH_RISKYMOD";
                }
            }
        }
    }

    public static class Skills
    {
        public static SkillDef PrepIceWall;
        public static SkillDef PrepFireStorm;
        public static SkillDef SpecialLightning;
        public static SkillDef SpecialLightningScepter;
        public static SkillDef SpecialFlamethrowerVanilla;
    }
}
