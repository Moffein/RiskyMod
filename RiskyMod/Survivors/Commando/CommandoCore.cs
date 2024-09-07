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
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Commando
{
    public class CommandoCore
    {
        public static bool enabled = true;

        public static bool removePrimaryFalloff = true;
        public static bool phaseRoundChanges = true;
        public static bool lightningRound = true;

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
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (removePrimaryFalloff)
            {
                IL.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += SharedHooks.BulletAttackHooks.RemoveBulletFalloff;
            }
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

            if (lightningRound)
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
                skillDef.icon = phaseRoundDef.icon; //TODO: GET UNIQUE ICON
                Content.Content.skillDefs.Add(phaseRoundDef);

                EntityStates.RiskyMod.Commando.FireLightningRound.lightningProjectilePrefab = BuildPhaseLightningProjectile();
                Skills.PhaseLightning = skillDef;
                SneedUtils.SneedUtils.AddSkillToFamily(Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Commando/CommandoBodySecondaryFamily.asset").WaitForCompletion(), Skills.PhaseLightning);
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
            if (suppressiveChanges)
            {
                SneedUtils.SneedUtils.DumpAddressableEntityStateConfig("RoR2/Junk/CommandoPerformanceTest/EntityStates.Commando.CommandoWeapon.FireBarrage.asset");
                SkillDef barrageDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyBarrage.asset").WaitForCompletion();
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Junk/CommandoPerformanceTest/EntityStates.Commando.CommandoWeapon.FireBarrage.asset", "baseBulletCount", "8");
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Junk/CommandoPerformanceTest/EntityStates.Commando.CommandoWeapon.FireBarrage.asset", "damageCoefficient", "1.2");
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Junk/CommandoPerformanceTest/EntityStates.Commando.CommandoWeapon.FireBarrage.asset", "baseDurationBetweenShots", "0.09");
                barrageDef.baseRechargeInterval = 7f;
                barrageDef.interruptPriority = InterruptPriority.PrioritySkill;
                barrageDef.mustKeyPress = false;
                barrageDef.skillDescriptionToken = "COMMANDO_SPECIAL_DESCRIPTION_RISKYMOD";
                IL.EntityStates.Commando.CommandoWeapon.FireBarrage.FireBullet += SharedHooks.BulletAttackHooks.RemoveBulletFalloff;
            }

            if (grenadeChanges)
            {
                GameObject moddedGrenade = BuildGrenadeProjectile();
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Commando.CommandoWeapon.ThrowGrenade", "projectilePrefab", moddedGrenade);
                ThrowGrenadeScepter.projectilePrefab = moddedGrenade;
            }

            if (SoftDependencies.ScepterPluginLoaded || SoftDependencies.ClassicItemsScepterLoaded)
            {
                BuildScepterSkillDefs(sk);
                if (SoftDependencies.ScepterPluginLoaded) SetupScepter();
                if (SoftDependencies.ClassicItemsScepterLoaded) SetupScepterClassic();
            }
        }

        private void BuildScepterSkillDefs(SkillLocator sk)
        {
            if (suppressiveChanges)
            {
                SuppressiveFireScepterDamage = DamageAPI.ReserveDamageType();
                OnHitAll.HandleOnHitAllActions += FireBarrageScepter.SuppressiveFireScepterAOE;
                SkillDef barrageDef = SkillDef.CreateInstance<SkillDef>();
                Content.Content.entityStates.Add(typeof(FireBarrageScepter));
                barrageDef.activationState = new SerializableEntityStateType(typeof(FireBarrageScepter));
                barrageDef.activationStateMachineName = "Weapon";
                barrageDef.baseMaxStock = 1;
                barrageDef.baseRechargeInterval = 7f;
                barrageDef.beginSkillCooldownOnSkillEnd = false;
                barrageDef.canceledFromSprinting = false;
                barrageDef.dontAllowPastMaxStocks = true;
                barrageDef.forceSprintDuringState = false;
                barrageDef.fullRestockOnAssign = true;
                barrageDef.icon = Content.Assets.ScepterSkillIcons.CommandoBarrageScepter;
                barrageDef.interruptPriority = InterruptPriority.PrioritySkill;
                barrageDef.isCombatSkill = true;
                barrageDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
                barrageDef.mustKeyPress = false;
                barrageDef.cancelSprintingOnActivation = true;
                barrageDef.rechargeStock = 1;
                barrageDef.requiredStock = 1;
                barrageDef.skillName = "BarrageScepter";
                barrageDef.skillNameToken = "COMMANDO_SPECIAL_SCEPTER_NAME_RISKYMOD";
                barrageDef.skillDescriptionToken = "COMMANDO_SPECIAL_SCEPTER_DESCRIPTION_RISKYMOD";
                barrageDef.stockToConsume = 1;
                Content.Content.skillDefs.Add(barrageDef);
                Skills.BarrageScepter = barrageDef;
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
            if (suppressiveChanges)
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.BarrageScepter, "CommandoBody", Skills.Barrage);
            }
            if (grenadeChanges)
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.GrenadeScepter, "CommandoBody", Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/ThrowGrenade.asset").WaitForCompletion());
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepterClassic()
        {
            if (suppressiveChanges)
            {
                ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.BarrageScepter, "CommandoBody", SkillSlot.Special, Skills.Barrage);
            }
            if (grenadeChanges)
            {
                ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.GrenadeScepter, "CommandoBody", SkillSlot.Special, Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/ThrowGrenade.asset").WaitForCompletion());
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
                DamageAPI.ModdedDamageTypeHolderComponent mdc = proj.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                mdc.Add(SharedDamageTypes.SweetSpotModifier);
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
        public static SkillDef Barrage;
        public static SkillDef BarrageScepter;
        public static SkillDef GrenadeScepter;
    }
}
