using EntityStates;
using EntityStates.RiskyMod.Commando;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Commando
{
    public class CommandoCore
    {
        public static bool enabled = true;
        public static bool enablePrimarySkillChanges = true;
        public static bool enableSecondarySkillChanges = true;
        public static bool enableUtilitySkillChanges = true;
        public static bool enableSpecialSkillChanges = true;
        public static DamageAPI.ModdedDamageType SuppressiveFireDamage;
        public CommandoCore()
        {
            if (!enabled) return;
            ModifySkills(RoR2Content.Survivors.Commando.bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySecondaries(sk);

            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (!enablePrimarySkillChanges) return;

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
            if (!enableSecondarySkillChanges) return;

            FirePhaseRound._projectilePrefab = BuildPhaseRoundProjectile();
            LoadoutAPI.AddSkill(typeof(FirePhaseRound));
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
            LoadoutAPI.AddSkillDef(phaseRoundDef);
            sk.secondary.skillFamily.variants[0].skillDef = phaseRoundDef;

            LoadoutAPI.AddSkill(typeof(FirePhaseBlast));
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
            phaseBlastDef.skillDescriptionToken = "COMMANDO_SECONDARY_ALT1_DESCRIPTION_RISKYMOD";
            phaseBlastDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(phaseBlastDef);
            sk.secondary.skillFamily.variants[1].skillDef = phaseBlastDef;
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (!enableSpecialSkillChanges) return;

            SuppressiveFireDamage = DamageAPI.ReserveDamageType();
            SkillDef barrageDef = SkillDef.CreateInstance<SkillDef>();
            barrageDef.activationState = new SerializableEntityStateType(typeof(FireBarrage));
            barrageDef.activationStateMachineName = "Weapon";
            barrageDef.baseMaxStock = 1;
            barrageDef.baseRechargeInterval = 6f;
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
            LoadoutAPI.AddSkillDef(barrageDef);
            sk.special.skillFamily.variants[0].skillDef = barrageDef;
            OnHitAll.HandleOnHitAllActions += FireBarrage.SuppressiveFireAOE;

            ThrowGrenade._projectilePrefab = BuildGrenadeProjectile();
            CookGrenade.overcookExplosionEffectPrefab = BuildGrenadeOvercookExplosionEffect();
            LoadoutAPI.AddSkill(typeof(CookGrenade));
            LoadoutAPI.AddSkill(typeof(ThrowGrenade));
            SkillDef grenadeDef = SkillDef.CreateInstance<SkillDef>();
            grenadeDef.activationState = new SerializableEntityStateType(typeof(CookGrenade));
            grenadeDef.activationStateMachineName = "Weapon";
            grenadeDef.baseMaxStock = 1;
            grenadeDef.baseRechargeInterval = 6f;
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
            LoadoutAPI.AddSkillDef(grenadeDef);
            sk.special.skillFamily.variants[1].skillDef = grenadeDef;
        }

        private GameObject BuildGrenadeProjectile()
        {
            GameObject proj = Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile").InstantiateClone("RiskyModFragProjectile", true);

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

            ProjectileAPI.Add(proj);
            return proj;
        }

        private GameObject BuildGrenadeOvercookExplosionEffect()
        {
            GameObject effect = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXCommandoGrenade").InstantiateClone("RiskyModFragOvercookEffect", false);
            EffectComponent ec = effect.GetComponent<EffectComponent>();
            ec.soundName = "Play_commando_M2_grenade_explo";
            EffectAPI.AddEffect(new EffectDef(effect));
            return effect;
        }

        private GameObject BuildPhaseRoundProjectile()
        {
            GameObject proj = Resources.Load<GameObject>("prefabs/projectiles/fmjramping").InstantiateClone("RiskyModPhaseRound", true);

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

            ProjectileAPI.Add(proj);
            return proj;
        }
    }
}
