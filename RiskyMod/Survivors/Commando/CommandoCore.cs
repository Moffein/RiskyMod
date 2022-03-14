using EntityStates;
using EntityStates.RiskyMod.Commando;
using EntityStates.RiskyMod.Commando.Scepter;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Commando
{
    public class CommandoCore
    {
        public static bool enabled = true;
        public static bool fixPrimaryFireRate = true;

        public static bool phaseRoundChanges = true;
        public static bool phaseBlastChanges = true;

        public static bool rollChanges = true;

        public static bool suppressiveChanges = true;
        public static bool grenadeChanges = true;

        public static DamageAPI.ModdedDamageType SuppressiveFireDamage;
        public static DamageAPI.ModdedDamageType SuppressiveFireScepterDamage;
        public static BuffDef SlideBuff; public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody");

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
            if (!fixPrimaryFireRate) return;

            //Removes the reload state completely since this messes with attack speed lategame.
            IL.EntityStates.Commando.CommandoWeapon.FirePistol2.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcI4(0)
                    );
                c.Index++;
                c.EmitDelegate<Func<int, int>>(zero =>
                {
                    return -1000;
                });
            };
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (phaseRoundChanges)
            {
                FirePhaseRound._projectilePrefab = BuildPhaseRoundProjectile();
                Content.Content.entityStates.Add(typeof(FirePhaseRound));
                SkillDef phaseRoundDef = SkillDef.CreateInstance<SkillDef>();
                phaseRoundDef.activationState = new SerializableEntityStateType(typeof(FirePhaseRound));
                phaseRoundDef.activationStateMachineName = "Weapon";
                phaseRoundDef.baseMaxStock = 1;
                phaseRoundDef.baseRechargeInterval = 3f;
                phaseRoundDef.beginSkillCooldownOnSkillEnd = false;
                phaseRoundDef.canceledFromSprinting = false;
                phaseRoundDef.dontAllowPastMaxStocks = true;
                phaseRoundDef.forceSprintDuringState = false;
                phaseRoundDef.fullRestockOnAssign = true;
                phaseRoundDef.icon = sk.secondary.skillFamily.variants[0].skillDef.icon;
                phaseRoundDef.interruptPriority = InterruptPriority.Skill;
                phaseRoundDef.isCombatSkill = true;
                phaseRoundDef.keywordTokens = new string[] { };
                phaseRoundDef.mustKeyPress = false;
                phaseRoundDef.cancelSprintingOnActivation = true;
                phaseRoundDef.rechargeStock = 1;
                phaseRoundDef.requiredStock = 1;
                phaseRoundDef.skillName = "FireFMJ";
                phaseRoundDef.skillNameToken = "COMMANDO_SECONDARY_NAME";
                phaseRoundDef.skillDescriptionToken = "COMMANDO_SECONDARY_DESCRIPTION_RISKYMOD";
                phaseRoundDef.stockToConsume = 1;
                SneedUtils.SneedUtils.FixSkillName(phaseRoundDef);
                Content.Content.skillDefs.Add(phaseRoundDef);
                sk.secondary.skillFamily.variants[0].skillDef = phaseRoundDef;
            }

            if (phaseBlastChanges)
            {
                Content.Content.entityStates.Add(typeof(FirePhaseBlast));
                SkillDef phaseBlastDef = SkillDef.CreateInstance<SkillDef>();
                phaseBlastDef.activationState = new SerializableEntityStateType(typeof(FirePhaseBlast));
                phaseBlastDef.activationStateMachineName = "Weapon";
                phaseBlastDef.baseMaxStock = 1;
                phaseBlastDef.baseRechargeInterval = 3f;
                phaseBlastDef.beginSkillCooldownOnSkillEnd = false;
                phaseBlastDef.canceledFromSprinting = false;
                phaseBlastDef.dontAllowPastMaxStocks = true;
                phaseBlastDef.forceSprintDuringState = false;
                phaseBlastDef.fullRestockOnAssign = true;
                phaseBlastDef.icon = sk.secondary.skillFamily.variants[1].skillDef.icon;
                phaseBlastDef.interruptPriority = InterruptPriority.Skill;
                phaseBlastDef.isCombatSkill = true;
                phaseBlastDef.keywordTokens = new string[] { };
                phaseBlastDef.mustKeyPress = false;
                phaseBlastDef.cancelSprintingOnActivation = true;
                phaseBlastDef.rechargeStock = 1;
                phaseBlastDef.requiredStock = 1;
                phaseBlastDef.skillName = "FireShotgunBlast";
                phaseBlastDef.skillNameToken = "COMMANDO_SECONDARY_ALT1_NAME";
                phaseBlastDef.skillDescriptionToken = "COMMANDO_SECONDARY_ALT1_DESCRIPTION";//_RISKYMOD
                phaseBlastDef.stockToConsume = 1;
                SneedUtils.SneedUtils.FixSkillName(phaseBlastDef);
                Content.Content.skillDefs.Add(phaseBlastDef);
                sk.secondary.skillFamily.variants[1].skillDef = phaseBlastDef;
            }
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (rollChanges)
            {
                sk.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "COMMANDO_UTILITY_DESCRIPTION_RISKYMOD";
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

        private void SlideStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(SlideBuff.buffIndex))
            {
                args.attackSpeedMultAdd += 0.5f;
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (suppressiveChanges)
            {
                SuppressiveFireDamage = DamageAPI.ReserveDamageType();
                Content.Content.entityStates.Add(typeof(FireBarrage));
                SkillDef barrageDef = SkillDef.CreateInstance<SkillDef>();
                barrageDef.activationState = new SerializableEntityStateType(typeof(FireBarrage));
                barrageDef.activationStateMachineName = "Weapon";
                barrageDef.baseMaxStock = 1;
                barrageDef.baseRechargeInterval = 7f;
                barrageDef.beginSkillCooldownOnSkillEnd = false;
                barrageDef.canceledFromSprinting = false;
                barrageDef.dontAllowPastMaxStocks = true;
                barrageDef.forceSprintDuringState = false;
                barrageDef.fullRestockOnAssign = true;
                barrageDef.icon = sk.special.skillFamily.variants[0].skillDef.icon;
                barrageDef.interruptPriority = InterruptPriority.PrioritySkill;
                barrageDef.isCombatSkill = true;
                barrageDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
                barrageDef.mustKeyPress = false;
                barrageDef.cancelSprintingOnActivation = true;
                barrageDef.rechargeStock = 1;
                barrageDef.requiredStock = 1;
                barrageDef.skillName = "Barrage";
                barrageDef.skillNameToken = "COMMANDO_SPECIAL_NAME";
                barrageDef.skillDescriptionToken = "COMMANDO_SPECIAL_DESCRIPTION_RISKYMOD";
                barrageDef.stockToConsume = 1;
                Content.Content.skillDefs.Add(barrageDef);
                sk.special.skillFamily.variants[0].skillDef = barrageDef;
                OnHitAll.HandleOnHitAllActions += FireBarrage.SuppressiveFireAOE;
            }

            if (grenadeChanges)
            {
                ThrowGrenade._projectilePrefab = BuildGrenadeProjectile();
                CookGrenade.overcookExplosionEffectPrefab = BuildGrenadeOvercookExplosionEffect();

                Content.Content.entityStates.Add(typeof(CookGrenade));
                Content.Content.entityStates.Add(typeof(ThrowGrenade));

                SkillDef grenadeDef = SkillDef.CreateInstance<SkillDef>();
                grenadeDef.activationState = new SerializableEntityStateType(typeof(CookGrenade));
                grenadeDef.activationStateMachineName = "Weapon";
                grenadeDef.baseMaxStock = 1;
                grenadeDef.baseRechargeInterval = 7f;
                grenadeDef.beginSkillCooldownOnSkillEnd = false;
                grenadeDef.canceledFromSprinting = false;
                grenadeDef.dontAllowPastMaxStocks = true;
                grenadeDef.forceSprintDuringState = false;
                grenadeDef.fullRestockOnAssign = true;
                grenadeDef.icon = sk.special.skillFamily.variants[1].skillDef.icon;
                grenadeDef.interruptPriority = InterruptPriority.PrioritySkill;
                grenadeDef.isCombatSkill = true;
                grenadeDef.keywordTokens = new string[] { };
                grenadeDef.mustKeyPress = false;
                grenadeDef.cancelSprintingOnActivation = true;
                grenadeDef.rechargeStock = 1;
                grenadeDef.requiredStock = 1;
                grenadeDef.skillName = "Grenade";
                grenadeDef.skillNameToken = "COMMANDO_SPECIAL_ALT1_NAME";
                grenadeDef.skillDescriptionToken = "COMMANDO_SPECIAL_ALT1_DESCRIPTION_RISKYMOD";
                grenadeDef.stockToConsume = 1;
                Content.Content.skillDefs.Add(grenadeDef);
                sk.special.skillFamily.variants[1].skillDef = grenadeDef;
            }

            if (RiskyMod.ScepterPluginLoaded)
            {
                SetupScepter(sk);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter(SkillLocator sk)
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
                barrageDef.baseRechargeInterval = 6f;
                barrageDef.beginSkillCooldownOnSkillEnd = false;
                barrageDef.canceledFromSprinting = false;
                barrageDef.dontAllowPastMaxStocks = true;
                barrageDef.forceSprintDuringState = false;
                barrageDef.fullRestockOnAssign = true;
                barrageDef.icon = AncientScepter.Assets.SpriteAssets.CommandoBarrage2;
                barrageDef.interruptPriority = InterruptPriority.PrioritySkill;
                barrageDef.isCombatSkill = true;
                barrageDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
                barrageDef.mustKeyPress = false;
                barrageDef.cancelSprintingOnActivation = true;
                barrageDef.rechargeStock = 1;
                barrageDef.requiredStock = 1;
                barrageDef.skillName = "BarrageScepter";
                barrageDef.skillNameToken = "COMMANDO_SPECIAL_SCEPTER_NAME";
                barrageDef.skillDescriptionToken = "COMMANDO_SPECIAL_SCEPTER_DESCRIPTION_RISKYMOD";
                barrageDef.stockToConsume = 1;
                Content.Content.skillDefs.Add(barrageDef);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(barrageDef, "CommandoBody", SkillSlot.Special, 0);
            }

            if (grenadeChanges)
            {
                ThrowGrenadeScepter._projectilePrefab = BuildGrenadeScepterProjectile();
                Content.Content.entityStates.Add(typeof(CookGrenadeScepter));
                Content.Content.entityStates.Add(typeof(ThrowGrenadeScepter));
                SkillDef grenadeDef = SkillDef.CreateInstance<SkillDef>();
                grenadeDef.activationState = new SerializableEntityStateType(typeof(CookGrenadeScepter));
                grenadeDef.activationStateMachineName = "Weapon";
                grenadeDef.baseMaxStock = 1;
                grenadeDef.baseRechargeInterval = 6f;
                grenadeDef.beginSkillCooldownOnSkillEnd = false;
                grenadeDef.canceledFromSprinting = false;
                grenadeDef.dontAllowPastMaxStocks = true;
                grenadeDef.forceSprintDuringState = false;
                grenadeDef.fullRestockOnAssign = true;
                grenadeDef.icon = AncientScepter.Assets.SpriteAssets.CommandoGrenade2;
                grenadeDef.interruptPriority = InterruptPriority.PrioritySkill;
                grenadeDef.isCombatSkill = true;
                grenadeDef.keywordTokens = new string[] { };
                grenadeDef.mustKeyPress = false;
                grenadeDef.cancelSprintingOnActivation = true;
                grenadeDef.rechargeStock = 1;
                grenadeDef.requiredStock = 1;
                grenadeDef.skillName = "GrenadeScepter";
                grenadeDef.skillNameToken = "COMMANDO_SPECIAL_ALT1_SCEPTER_NAME";
                grenadeDef.skillDescriptionToken = "COMMANDO_SPECIAL_ALT1_SCEPTER_DESCRIPTION_RISKYMOD";
                grenadeDef.stockToConsume = 1;
                Content.Content.skillDefs.Add(grenadeDef);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(grenadeDef, "CommandoBody", SkillSlot.Special, 1);
            }
        }
        private GameObject BuildGrenadeScepterProjectile()
        {
            GameObject proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile").InstantiateClone("RiskyModFragScepterProjectile", true);

            ProjectileSimple ps = proj.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;

            ProjectileImpactExplosion pie = proj.GetComponent<ProjectileImpactExplosion>();
            pie.timerAfterImpact = false;
            pie.lifetime = CookGrenade.totalFuseTime;
            pie.blastRadius = CookGrenadeScepter.selfBlastRadius;
            pie.falloffModel = BlastAttack.FalloffModel.None;

            ProjectileDamage pd = proj.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            proj.AddComponent<GrenadeTimer>();

            Content.Content.projectilePrefabs.Add(proj);
            return proj;
        }
        private GameObject BuildGrenadeProjectile()
        {
            GameObject proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile").InstantiateClone("RiskyModFragProjectile", true);

            ProjectileSimple ps = proj.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;

            ProjectileImpactExplosion pie = proj.GetComponent<ProjectileImpactExplosion>();
            pie.timerAfterImpact = false;
            pie.lifetime = CookGrenade.totalFuseTime;
            pie.blastRadius = CookGrenade.selfBlastRadius;
            pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;

            ProjectileDamage pd = proj.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            proj.AddComponent<GrenadeTimer>();

            Content.Content.projectilePrefabs.Add(proj);
            return proj;
        }
        private GameObject BuildGrenadeOvercookExplosionEffect()
        {
            GameObject effect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXCommandoGrenade").InstantiateClone("RiskyModFragOvercookEffect", false);
            EffectComponent ec = effect.GetComponent<EffectComponent>();
            ec.soundName = "Play_commando_M2_grenade_explo";
            Content.Content.effectDefs.Add(new EffectDef(effect));
            return effect;
        }
        private GameObject BuildPhaseRoundProjectile()
        {
            GameObject proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/fmjramping").InstantiateClone("RiskyModPhaseRound", true);

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
            pbc.damageCoefficient = 0.5f;
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
    }
}
