using EntityStates;
using EntityStates.RiskyMod.Commando;
using EntityStates.RiskyMod.Commando.Scepter;
using MonoMod.Cil;
using R2API;
using RiskyMod.Content;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.Skills.SkillFamily;

namespace RiskyMod.Survivors.Commando
{
    public class CommandoCore
    {
        public static bool enabled = true;

        public static bool phaseRoundChanges = true;
        public static bool skillLightningRound = true;
        public static bool skillShrapnelBarrage = true;

        public static bool replacePhaseRound = false;
        public static bool replaceSuppressive = false;

        public static bool rollChanges = true;

        public static bool suppressiveChanges = true;
        public static bool grenadeChanges = true;
        public static bool grenadeRemoveFalloff = true;
        
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody");

        public CommandoCore()
        {
            if (!enabled) return;
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            SkillDef phaseRoundDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFireFMJ.asset").WaitForCompletion();
            SkillDef phaseBlastDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFireShotgunBlast.asset").WaitForCompletion();
            if (phaseRoundChanges)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Commando.CommandoWeapon.FireFMJ", "projectilePrefab", BuildPhaseRoundProjectile());
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Commando.CommandoWeapon.FireFMJ", "damageCoefficient", "4.5");
                phaseRoundDef.skillDescriptionToken = "COMMANDO_SECONDARY_DESCRIPTION_RISKYMOD";
            }

            phaseRoundDef.mustKeyPress = false;
            phaseBlastDef.mustKeyPress = false;

            if (skillLightningRound)
            {
                Content.Content.entityStates.Add(typeof(FireLightningRound));
                SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
                skillDef.activationState = new SerializableEntityStateType(typeof(FireLightningRound));
                skillDef.activationStateMachineName = "Weapon";
                skillDef.attackSpeedBuffsRestockSpeed = false;
                skillDef.cancelSprintingOnActivation = true;
                skillDef.dontAllowPastMaxStocks = true;
                skillDef.fullRestockOnAssign = true;
                skillDef.baseMaxStock = 1;
                skillDef.baseRechargeInterval = 3f;
                skillDef.beginSkillCooldownOnSkillEnd = false;
                skillDef.canceledFromSprinting = false;
                skillDef.isCombatSkill = true;
                skillDef.interruptPriority = InterruptPriority.Skill;
                skillDef.mustKeyPress = false;
                skillDef.skillName = "RiskyModLightningRound";
                (skillDef as ScriptableObject).name = skillDef.skillName;
                skillDef.skillNameToken = "COMMANDO_SECONDARY_LIGHTNING_NAME_RISKYMOD";
                skillDef.skillDescriptionToken = "COMMANDO_SECONDARY_LIGHTNING_DESCRIPTION_RISKYMOD";
                skillDef.icon = phaseRoundDef.icon;
                if (!replacePhaseRound)
                {
                    skillDef.icon = Content.Assets.assetBundle.LoadAsset<Sprite>("CommandoLightningRound.png");
                }
                Content.Content.skillDefs.Add(skillDef);

                EntityStates.RiskyMod.Commando.FireLightningRound.lightningProjectilePrefab = BuildPhaseLightningProjectile();
                Skills.PhaseLightning = skillDef;

                if (replacePhaseRound)
                {
                    SkillFamily secondaryFamily = Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Commando/CommandoBodySecondaryFamily.asset").WaitForCompletion();
                    List<SkillFamily.Variant> variantsList = secondaryFamily.variants.Where(variant => variant.skillDef != phaseRoundDef).ToList();

                    var variant = new SkillFamily.Variant
                    {
                        skillDef = skillDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
                    };
                    variantsList.Insert(0, variant);
                    secondaryFamily.variants = variantsList.ToArray();
                }
                else
                {
                    SneedUtils.SneedUtils.AddSkillToFamily(Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Commando/CommandoBodySecondaryFamily.asset").WaitForCompletion(), Skills.PhaseLightning);
                }
                
            }
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (rollChanges)
            {
                SkillDef rollDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyRoll.asset").WaitForCompletion();
                rollDef.skillDescriptionToken = "COMMANDO_UTILITY_DESCRIPTION_RISKYMOD";
                On.EntityStates.Commando.DodgeState.OnEnter += (orig, self) =>
                {
                    orig(self);
                    if (self.isAuthority && self.skillLocator)
                    {
                        self.skillLocator.primary.RunRecharge(1f);
                        self.skillLocator.secondary.RunRecharge(1f);
                        self.skillLocator.special.RunRecharge(1f);
                    }
                };
                //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Commando.DodgeState");  //base speed is 5/2.5
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Commando.DodgeState", "initialSpeedCoefficient", "7.25");
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Commando.DodgeState", "finalSpeedCoefficient", "3.625");
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            SkillDef barrageDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyBarrage.asset").WaitForCompletion();
            if (suppressiveChanges)
            {
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Junk/CommandoPerformanceTest/EntityStates.Commando.CommandoWeapon.FireBarrage.asset", "baseBulletCount", "8");
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Junk/CommandoPerformanceTest/EntityStates.Commando.CommandoWeapon.FireBarrage.asset", "damageCoefficient", "1.2");
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Junk/CommandoPerformanceTest/EntityStates.Commando.CommandoWeapon.FireBarrage.asset", "baseDurationBetweenShots", "0.09");
                barrageDef.baseRechargeInterval = 7f;
                barrageDef.interruptPriority = InterruptPriority.PrioritySkill;
                barrageDef.mustKeyPress = false;
                barrageDef.skillDescriptionToken = "COMMANDO_SPECIAL_DESCRIPTION_RISKYMOD";
                IL.EntityStates.Commando.CommandoWeapon.FireBarrage.FireBullet += SharedHooks.BulletAttackHooks.RemoveBulletFalloff;

                On.EntityStates.Commando.CommandoWeapon.FireBarrage.GetMinimumInterruptPriority += FireBarrage_GetMinimumInterruptPriority;
            }

            if (grenadeChanges)
            {
                GameObject moddedGrenade = BuildGrenadeProjectile();
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Commando.CommandoWeapon.ThrowGrenade", "projectilePrefab", moddedGrenade);
                ThrowGrenadeScepter.projectilePrefab = moddedGrenade;
            }

            if (skillShrapnelBarrage)
            {
                SkillDef shrapnelBarrageDef = SkillDef.CreateInstance<SkillDef>();
                Content.Content.entityStates.Add(typeof(ShrapnelBarrage));
                shrapnelBarrageDef.activationState = new SerializableEntityStateType(typeof(ShrapnelBarrage));
                shrapnelBarrageDef.activationStateMachineName = "Weapon";
                shrapnelBarrageDef.baseMaxStock = 1;
                shrapnelBarrageDef.baseRechargeInterval = 7f;
                shrapnelBarrageDef.beginSkillCooldownOnSkillEnd = false;
                shrapnelBarrageDef.canceledFromSprinting = false;
                shrapnelBarrageDef.dontAllowPastMaxStocks = true;
                shrapnelBarrageDef.forceSprintDuringState = false;
                shrapnelBarrageDef.fullRestockOnAssign = true;
                shrapnelBarrageDef.icon = barrageDef.icon;
                if (!replaceSuppressive)
                {
                    shrapnelBarrageDef.icon = Content.Assets.assetBundle.LoadAsset<Sprite>("CommandoShrapnelBarrage.png");
                }
                shrapnelBarrageDef.interruptPriority = InterruptPriority.PrioritySkill;
                shrapnelBarrageDef.isCombatSkill = true;
                shrapnelBarrageDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
                shrapnelBarrageDef.mustKeyPress = false;
                shrapnelBarrageDef.cancelSprintingOnActivation = true;
                shrapnelBarrageDef.rechargeStock = 1;
                shrapnelBarrageDef.requiredStock = 1;
                shrapnelBarrageDef.skillName = "RiskyModShrapnelBarrage";
                shrapnelBarrageDef.skillNameToken = "COMMANDO_SPECIAL_EXPL_NAME_RISKYMOD";
                shrapnelBarrageDef.skillDescriptionToken = "COMMANDO_SPECIAL_EXPL_DESCRIPTION_RISKYMOD";
                shrapnelBarrageDef.stockToConsume = 1;
                (shrapnelBarrageDef as ScriptableObject).name = shrapnelBarrageDef.skillName;
                Content.Content.skillDefs.Add(shrapnelBarrageDef);
                Skills.ShrapnelBarrage = shrapnelBarrageDef;

                if (replaceSuppressive)
                {
                    SkillFamily specialFamily = Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Commando/CommandoBodySpecialFamily.asset").WaitForCompletion();
                    List<SkillFamily.Variant> variantsList = specialFamily.variants.Where(variant => variant.skillDef != barrageDef).ToList();

                    var variant = new SkillFamily.Variant
                    {
                        skillDef = shrapnelBarrageDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(shrapnelBarrageDef.skillNameToken, false, null)
                    };
                    variantsList.Insert(0, variant);
                    specialFamily.variants = variantsList.ToArray();
                }
                else
                {
                    SneedUtils.SneedUtils.AddSkillToFamily(Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Commando/CommandoBodySpecialFamily.asset").WaitForCompletion(), shrapnelBarrageDef);
                }
            }

            if (SoftDependencies.ScepterPluginLoaded)
            {
                BuildScepterSkillDefs(sk);
                SetupScepter();
            }
        }

        private InterruptPriority FireBarrage_GetMinimumInterruptPriority(On.EntityStates.Commando.CommandoWeapon.FireBarrage.orig_GetMinimumInterruptPriority orig, EntityStates.Commando.CommandoWeapon.FireBarrage self)
        {
            if (self.inputBank && self.inputBank.skill4.down) return InterruptPriority.Pain;
            return InterruptPriority.Skill;
        }

        private void BuildScepterSkillDefs(SkillLocator sk)
        {
            if (skillShrapnelBarrage)
            {
                SkillDef shrapnelBarrageDef = SkillDef.CreateInstance<SkillDef>();
                Content.Content.entityStates.Add(typeof(ShrapnelBarrageScepter));
                shrapnelBarrageDef.activationState = new SerializableEntityStateType(typeof(ShrapnelBarrageScepter));
                shrapnelBarrageDef.activationStateMachineName = "Weapon";
                shrapnelBarrageDef.baseMaxStock = 1;
                shrapnelBarrageDef.baseRechargeInterval = 7f;
                shrapnelBarrageDef.beginSkillCooldownOnSkillEnd = false;
                shrapnelBarrageDef.canceledFromSprinting = false;
                shrapnelBarrageDef.dontAllowPastMaxStocks = true;
                shrapnelBarrageDef.forceSprintDuringState = false;
                shrapnelBarrageDef.fullRestockOnAssign = true;
                shrapnelBarrageDef.icon = Content.Assets.ScepterSkillIcons.CommandoBarrageScepter;
                if (!replaceSuppressive)
                {
                    shrapnelBarrageDef.icon = Content.Assets.assetBundle.LoadAsset<Sprite>("CommandoShrapnelBarrageScepter.png");
                }
                shrapnelBarrageDef.interruptPriority = InterruptPriority.PrioritySkill;
                shrapnelBarrageDef.isCombatSkill = true;
                shrapnelBarrageDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
                shrapnelBarrageDef.mustKeyPress = false;
                shrapnelBarrageDef.cancelSprintingOnActivation = true;
                shrapnelBarrageDef.rechargeStock = 1;
                shrapnelBarrageDef.requiredStock = 1;
                shrapnelBarrageDef.skillName = "RiskyModShrapnelBarrageScepter";
                shrapnelBarrageDef.skillNameToken = "COMMANDO_SPECIAL_EXPL_SCEPTER_NAME_RISKYMOD";
                shrapnelBarrageDef.skillDescriptionToken = "COMMANDO_SPECIAL_EXPL_SCEPTER_DESCRIPTION_RISKYMOD";
                shrapnelBarrageDef.stockToConsume = 1;
                (shrapnelBarrageDef as ScriptableObject).name = shrapnelBarrageDef.skillName;
                Content.Content.skillDefs.Add(shrapnelBarrageDef);
                Skills.ShrapnelBarrageScepter = shrapnelBarrageDef;
            }

            if (grenadeChanges)
            {
                SkillDef grenadeDef = SkillDef.CreateInstance<SkillDef>();
                Content.Content.entityStates.Add(typeof(ThrowGrenadeScepter));
                grenadeDef.activationState = new SerializableEntityStateType(typeof(ThrowGrenadeScepter));
                grenadeDef.activationStateMachineName = "Weapon";
                grenadeDef.baseMaxStock = 2;
                grenadeDef.baseRechargeInterval = 5f;
                grenadeDef.beginSkillCooldownOnSkillEnd = false;
                grenadeDef.canceledFromSprinting = false;
                grenadeDef.dontAllowPastMaxStocks = true;
                grenadeDef.forceSprintDuringState = false;
                grenadeDef.fullRestockOnAssign = true;
                grenadeDef.icon = Content.Assets.ScepterSkillIcons.CommandoGrenadeScepter;
                grenadeDef.interruptPriority = InterruptPriority.PrioritySkill;
                grenadeDef.isCombatSkill = true;
                grenadeDef.keywordTokens = new string[] {};
                grenadeDef.mustKeyPress = true;
                grenadeDef.cancelSprintingOnActivation = true;
                grenadeDef.rechargeStock = 1;
                grenadeDef.requiredStock = 1;
                grenadeDef.skillName = "GrenadeScepter";
                grenadeDef.skillNameToken = "COMMANDO_SPECIAL_ALT1_SCEPTER_NAME_RISKYMOD";
                grenadeDef.skillDescriptionToken = "COMMANDO_SPECIAL_ALT1_SCEPTER_DESCRIPTION_RISKYMOD";
                grenadeDef.stockToConsume = 1;
                Content.Content.skillDefs.Add(grenadeDef);
                Skills.GrenadeScepter = grenadeDef;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            if (skillShrapnelBarrage)
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.ShrapnelBarrageScepter, "CommandoBody", Skills.ShrapnelBarrage);
            }
            if (grenadeChanges)
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.GrenadeScepter, "CommandoBody", Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/ThrowGrenade.asset").WaitForCompletion());
            }
        }

        private GameObject BuildGrenadeProjectile()
        {
            GameObject proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile").InstantiateClone("RiskyModFragProjectile", true);

            ProjectileImpactExplosion pie = proj.GetComponent<ProjectileImpactExplosion>();
            pie.destroyOnWorld = false;
            pie.destroyOnEnemy = true;
            pie.blastRadius = 12f;

            if (grenadeRemoveFalloff)
            {
                pie.falloffModel = BlastAttack.FalloffModel.None;
            }
            else
            {
                pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                ProjectileDamage pd = proj.GetComponent<ProjectileDamage>();
                pd.damageType.AddModdedDamageType(SharedDamageTypes.SweetSpotModifier);
            }

            proj.AddComponent<GrenadeImpactComponent>();

            Content.Content.projectilePrefabs.Add(proj);
            return proj;
        }

        private GameObject BuildPhaseLightningProjectile()
        {
            GameObject proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/fmjramping").InstantiateClone("RiskyModPhaseLightning", true);
            //Add Lightning
            ProjectileProximityBeamController pbc = proj.GetComponent<ProjectileProximityBeamController>();
            if (!pbc)
            {
                pbc = proj.AddComponent<ProjectileProximityBeamController>();
            }
            pbc.attackFireCount = 1;
            pbc.attackInterval = 0.06f;
            pbc.attackRange = 10f;
            pbc.listClearInterval = 10f;
            pbc.minAngleFilter = 0f;
            pbc.maxAngleFilter = 180f;
            pbc.procCoefficient = 0.5f;
            pbc.damageCoefficient = 0.3333333333f;
            pbc.bounces = 0;
            pbc.lightningType = RoR2.Orbs.LightningOrb.LightningType.Ukulele;

            //Prevents projectiles from disappearing at long range
            ProjectileSimple ps = proj.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;

            ProjectileOverlapAttack poa = proj.GetComponent<ProjectileOverlapAttack>();
            poa.onServerHit = null;

            Content.Content.projectilePrefabs.Add(proj);
            return proj;
        }

        private GameObject BuildPhaseRoundProjectile()
        {
            GameObject proj = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRamping.prefab").WaitForCompletion().InstantiateClone("RiskyModPhaseRound", true);
            GameObject projGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRampingGhost.prefab").WaitForCompletion().InstantiateClone("RiskyModPhaseRoundGhost", false);

            //Make bigger
            proj.transform.localScale = Vector3.one * 2f;
            projGhost.transform.localScale = Vector3.one * 2f;

            proj.GetComponent<ProjectileController>().ghostPrefab = projGhost;

            //Prevents projectiles from disappearing at long range
            ProjectileSimple ps = proj.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;

            Content.Content.projectilePrefabs.Add(proj);
            return proj;
        }
    }

    public class Skills
    {
        public static SkillDef PhaseLightning;
        public static SkillDef ShrapnelBarrage;
        public static SkillDef ShrapnelBarrageScepter;
        public static SkillDef GrenadeScepter;
    }
}
