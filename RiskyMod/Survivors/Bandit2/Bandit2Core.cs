using EntityStates;
using EntityStates.RiskyMod.Bandit2;
using EntityStates.RiskyMod.Bandit2.Primary;
using EntityStates.RiskyMod.Bandit2.Revolver;
using EntityStates.RiskyMod.Bandit2.Revolver.Scepter;
using MonoMod.RuntimeDetour;
using R2API;
using R2API.Utils;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class Bandit2Core
    {
        public static BuffDef SpecialDebuff;
        public static DamageAPI.ModdedDamageType SpecialDamage;
        public static DamageAPI.ModdedDamageType RackEmUpDamage;
        public static bool enabled = true;

        public static bool backstabNerf = true;

        public static bool blastChanges = true;
        public static bool burstChanges = true;

        public static bool knifeChanges = true;
        public static bool knifeThrowChanges = true;

        public static bool utilityFix = true;

        public static bool specialRework = true;

        public static BodyIndex Bandit2Index;
        private static AnimationCurve knifeVelocity;

        public Bandit2Core()
        {
            if (!enabled) return;
            new BanditSpecialGracePeriod();
            new DesperadoRework();

            ModifySkills(RoR2Content.Survivors.Bandit2.bodyPrefab.GetComponent<SkillLocator>());

            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                Bandit2Index = BodyCatalog.FindBodyIndex("Bandit2Body");
            };
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPassives(sk);
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPassives(SkillLocator sk)
        {
            if (!backstabNerf) return;
            new BackstabRework();
            sk.passiveSkill.skillDescriptionToken = "BANDIT2_PASSIVE_DESCRIPTION_RISKYMOD";
        }

        private void ModifyPrimaries(SkillLocator sk)
        {

            LoadoutAPI.AddSkill(typeof(EnterReload));
            LoadoutAPI.AddSkill(typeof(Reload));

            if (burstChanges)
            {
                LoadoutAPI.AddSkill(typeof(FirePrimaryShotgun));
                ReloadSkillDef shotgunDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
                shotgunDef.activationState = new SerializableEntityStateType(typeof(FirePrimaryShotgun));
                shotgunDef.activationStateMachineName = "Weapon";
                shotgunDef.baseMaxStock = 4;
                shotgunDef.baseRechargeInterval = 0f;
                shotgunDef.beginSkillCooldownOnSkillEnd = false;
                shotgunDef.canceledFromSprinting = false;
                shotgunDef.dontAllowPastMaxStocks = true;
                shotgunDef.forceSprintDuringState = false;
                shotgunDef.fullRestockOnAssign = true;
                shotgunDef.icon = sk.primary._skillFamily.variants[0].skillDef.icon;
                shotgunDef.interruptPriority = InterruptPriority.Skill;
                shotgunDef.isCombatSkill = true;
                shotgunDef.keywordTokens = new string[] { };
                shotgunDef.mustKeyPress = false;
                shotgunDef.cancelSprintingOnActivation = true;
                shotgunDef.rechargeStock = 0;
                shotgunDef.requiredStock = 1;
                shotgunDef.skillName = "FireShotgun";
                shotgunDef.skillNameToken = "BANDIT2_PRIMARY_NAME";
                shotgunDef.skillDescriptionToken = "BANDIT2_PRIMARY_DESC_RISKYMOD";
                shotgunDef.stockToConsume = 1;
                shotgunDef.graceDuration = 0.4f;
                shotgunDef.reloadState = new SerializableEntityStateType(typeof(EnterReload));
                shotgunDef.reloadInterruptPriority = InterruptPriority.Any;
                LoadoutAPI.AddSkillDef(shotgunDef);
                sk.primary._skillFamily.variants[0].skillDef = shotgunDef;
            }

            if (blastChanges)
            {
                LoadoutAPI.AddSkill(typeof(FirePrimaryRifle));
                ReloadSkillDef rifleDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
                rifleDef.activationState = new SerializableEntityStateType(typeof(FirePrimaryRifle));
                rifleDef.activationStateMachineName = "Weapon";
                rifleDef.baseMaxStock = 4;
                rifleDef.baseRechargeInterval = 0f;
                rifleDef.beginSkillCooldownOnSkillEnd = false;
                rifleDef.canceledFromSprinting = false;
                rifleDef.dontAllowPastMaxStocks = true;
                rifleDef.forceSprintDuringState = false;
                rifleDef.fullRestockOnAssign = true;
                rifleDef.icon = sk.primary._skillFamily.variants[1].skillDef.icon;
                rifleDef.interruptPriority = InterruptPriority.Skill;
                rifleDef.isCombatSkill = true;
                rifleDef.keywordTokens = new string[] { };
                rifleDef.mustKeyPress = false;
                rifleDef.cancelSprintingOnActivation = true;
                rifleDef.rechargeStock = 0;
                rifleDef.requiredStock = 1;
                rifleDef.skillName = "FireRifle";
                rifleDef.skillNameToken = "BANDIT2_PRIMARY_ALT_NAME";
                rifleDef.skillDescriptionToken = "BANDIT2_PRIMARY_ALT_DESC_RISKYMOD";
                rifleDef.stockToConsume = 1;
                rifleDef.graceDuration = 0.4f;
                rifleDef.reloadState = new SerializableEntityStateType(typeof(EnterReload));
                rifleDef.reloadInterruptPriority = InterruptPriority.Any;
                LoadoutAPI.AddSkillDef(rifleDef);
                sk.primary._skillFamily.variants[1].skillDef = rifleDef;
            }
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (knifeChanges)
            {
                new IncreaseKnifeHitboxSize();
                knifeVelocity = BuildSlashVelocityCurve();
                sk.secondary.skillFamily.variants[0].skillDef.canceledFromSprinting = false;

                On.EntityStates.Bandit2.Weapon.SlashBlade.OnEnter += (orig, self) =>
                {
                    orig(self);
                    if (self.characterBody && self.characterBody.isSprinting)
                    {
                        self.ignoreAttackSpeed = true;
                        self.forceForwardVelocity = true;
                        self.forwardVelocityCurve = knifeVelocity;
                    }
                };

                SneedUtils.SneedUtils.SetEntityStateField("entitystates.bandit2.weapon.slashblade", "ignoreAttackSpeed", "1");
                var getBandit2SlashBladeMinDuration = new Hook(typeof(EntityStates.Bandit2.Weapon.SlashBlade).GetMethodCached("get_minimumDuration"), typeof(Bandit2Core).GetMethodCached(nameof(GetBandit2SlashBladeMinDurationHook)));
            }

            if (knifeThrowChanges)
            {
                sk.secondary.skillFamily.variants[1].skillDef.keywordTokens = new string[] { "KEYWORD_STUNNING", "KEYWORD_SUPERBLEED" };
                sk.secondary.skillFamily.variants[1].skillDef.skillDescriptionToken = "BANDIT2_SECONDARY_ALT_DESCRIPTION_RISKYMOD";

                On.EntityStates.Bandit2.Weapon.Bandit2FireShiv.FireShiv += (orig, self) =>
                {
                    if (EntityStates.Bandit2.Weapon.Bandit2FireShiv.muzzleEffectPrefab)
                    {
                        EffectManager.SimpleMuzzleFlash(EntityStates.Bandit2.Weapon.Bandit2FireShiv.muzzleEffectPrefab, self.gameObject, EntityStates.Bandit2.Weapon.Bandit2FireShiv.muzzleString, false);
                    }
                    if (self.isAuthority)
                    {
                        Ray aimRay = self.GetAimRay();
                        if (self.projectilePrefab != null)
                        {
                            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                            {
                                projectilePrefab = self.projectilePrefab,
                                position = aimRay.origin,
                                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                                owner = self.gameObject,
                                damage = self.damageStat * self.damageCoefficient,
                                force = self.force,
                                crit = self.RollCrit(),
                                damageTypeOverride = new DamageType?(DamageType.SuperBleedOnCrit | DamageType.Stun1s)
                            };
                            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                        }
                    }
                };
            }
        }

        private static float GetBandit2SlashBladeMinDurationHook(EntityStates.Bandit2.Weapon.SlashBlade self)
        {
            return 0.3f;
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (!utilityFix) return;

            LoadoutAPI.AddSkill(typeof(ThrowSmokebomb));
            LoadoutAPI.AddSkill(typeof(StealthMode));
            SkillDef stealthDef = SkillDef.CreateInstance<SkillDef>();
            stealthDef.activationState = new SerializableEntityStateType(typeof(ThrowSmokebomb));
            stealthDef.activationStateMachineName = "Stealth";
            stealthDef.baseMaxStock = 1;
            stealthDef.baseRechargeInterval = 6f;
            stealthDef.beginSkillCooldownOnSkillEnd = false;
            stealthDef.canceledFromSprinting = false;
            stealthDef.forceSprintDuringState = true;
            stealthDef.dontAllowPastMaxStocks = true;
            stealthDef.fullRestockOnAssign = true;
            stealthDef.icon = sk.utility._skillFamily.variants[0].skillDef.icon;
            stealthDef.interruptPriority = InterruptPriority.Skill;
            stealthDef.isCombatSkill = false;
            stealthDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            stealthDef.mustKeyPress = false;
            stealthDef.cancelSprintingOnActivation = false;
            stealthDef.rechargeStock = 1;
            stealthDef.requiredStock = 1;
            stealthDef.skillName = "Stealth";
            stealthDef.skillNameToken = "BANDIT2_UTILITY_NAME";
            stealthDef.skillDescriptionToken = "BANDIT2_UTILITY_DESCRIPTION";
            stealthDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(stealthDef);
            sk.utility._skillFamily.variants[0].skillDef = stealthDef;
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (!specialRework) return;
            SpecialDamage = DamageAPI.ReserveDamageType();
            RackEmUpDamage = DamageAPI.ReserveDamageType();
            SpecialDamageType(sk);
            SpecialDebuff = BuildSpecialDebuff();
            new SpecialDamageTweaks();

            LoadoutAPI.AddSkill(typeof(BaseSidearmState));
            LoadoutAPI.AddSkill(typeof(ExitSidearm));

            SkillDef lightsOutDef = SkillDef.CreateInstance<SkillDef>();
            LoadoutAPI.AddSkill(typeof(PrepLightsOut));
            LoadoutAPI.AddSkill(typeof(FireLightsOut));
            lightsOutDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOut));
            lightsOutDef.activationStateMachineName = "Weapon";
            lightsOutDef.baseMaxStock = 1;
            lightsOutDef.baseRechargeInterval = 4f;
            lightsOutDef.beginSkillCooldownOnSkillEnd = true;
            lightsOutDef.canceledFromSprinting = false;
            lightsOutDef.forceSprintDuringState = false;
            lightsOutDef.dontAllowPastMaxStocks = true;
            lightsOutDef.fullRestockOnAssign = true;
            lightsOutDef.icon = sk.special._skillFamily.variants[0].skillDef.icon;
            lightsOutDef.interruptPriority = InterruptPriority.Skill;
            lightsOutDef.isCombatSkill = true;
            lightsOutDef.keywordTokens = new string[] { "KEYWORD_SLAYER" };
            lightsOutDef.mustKeyPress = false;
            lightsOutDef.cancelSprintingOnActivation = true;
            lightsOutDef.rechargeStock = 1;
            lightsOutDef.requiredStock = 1;
            lightsOutDef.skillName = "LightsOut";
            lightsOutDef.skillNameToken = "BANDIT2_SPECIAL_NAME";
            lightsOutDef.skillDescriptionToken = "BANDIT2_SPECIAL_DESCRIPTION_RISKYMOD";
            lightsOutDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(lightsOutDef);
            sk.special._skillFamily.variants[0].skillDef = lightsOutDef;

            SkillDef reuDef = SkillDef.CreateInstance<SkillDef>();
            LoadoutAPI.AddSkill(typeof(PrepRackEmUp));
            LoadoutAPI.AddSkill(typeof(FireRackEmUp));
            reuDef.activationState = new SerializableEntityStateType(typeof(PrepRackEmUp));
            reuDef.activationStateMachineName = "Weapon";
            reuDef.baseMaxStock = 1;
            reuDef.baseRechargeInterval = 4f;
            reuDef.beginSkillCooldownOnSkillEnd = true;
            reuDef.canceledFromSprinting = false;
            reuDef.forceSprintDuringState = false;
            reuDef.dontAllowPastMaxStocks = true;
            reuDef.fullRestockOnAssign = true;
            reuDef.icon = sk.special._skillFamily.variants[1].skillDef.icon;
            reuDef.interruptPriority = InterruptPriority.Skill;
            reuDef.isCombatSkill = true;
            reuDef.keywordTokens = new string[] { "KEYWORD_SLAYER" };
            reuDef.mustKeyPress = false;
            reuDef.cancelSprintingOnActivation = true;
            reuDef.rechargeStock = 1;
            reuDef.requiredStock = 1;
            reuDef.skillName = "RackEmUp";
            reuDef.skillNameToken = "BANDIT2_SPECIAL_ALT_NAME_RISKYMOD";
            reuDef.skillDescriptionToken = "BANDIT2_SPECIAL_ALT_DESCRIPTION_RISKYMOD";
            reuDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(reuDef);
            sk.special._skillFamily.variants[1].skillDef = reuDef;

            if (RiskyMod.ScepterPluginLoaded)
            {
                SetupScepter(sk);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter(SkillLocator sk)
        {
            SkillDef lightsOutDef = SkillDef.CreateInstance<SkillDef>();
            LoadoutAPI.AddSkill(typeof(PrepLightsOutScepter));
            LoadoutAPI.AddSkill(typeof(FireLightsOutScepter));
            lightsOutDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOutScepter));
            lightsOutDef.activationStateMachineName = "Weapon";
            lightsOutDef.baseMaxStock = 1;
            lightsOutDef.baseRechargeInterval = 4f;
            lightsOutDef.beginSkillCooldownOnSkillEnd = true;
            lightsOutDef.canceledFromSprinting = false;
            lightsOutDef.forceSprintDuringState = false;
            lightsOutDef.dontAllowPastMaxStocks = true;
            lightsOutDef.fullRestockOnAssign = true;
            lightsOutDef.icon = AncientScepter.Assets.SpriteAssets.Bandit2ResetRevolver2;
            lightsOutDef.interruptPriority = InterruptPriority.Skill;
            lightsOutDef.isCombatSkill = true;
            lightsOutDef.keywordTokens = new string[] { "KEYWORD_SLAYER" };
            lightsOutDef.mustKeyPress = false;
            lightsOutDef.cancelSprintingOnActivation = true;
            lightsOutDef.rechargeStock = 1;
            lightsOutDef.requiredStock = 1;
            lightsOutDef.skillName = "LightsOutScepter";
            lightsOutDef.skillNameToken = "BANDIT2_SPECIAL_SCEPTER_NAME_RISKYMOD";
            lightsOutDef.skillDescriptionToken = "BANDIT2_SPECIAL_SCEPTER_DESCRIPTION_RISKYMOD";
            lightsOutDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(lightsOutDef);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(lightsOutDef, "Bandit2Body", SkillSlot.Special, 0);

            SkillDef reuDef = SkillDef.CreateInstance<SkillDef>();
            LoadoutAPI.AddSkill(typeof(PrepRackEmUpScepter));
            LoadoutAPI.AddSkill(typeof(FireRackEmUpScepter));
            reuDef.activationState = new SerializableEntityStateType(typeof(PrepRackEmUpScepter));
            reuDef.activationStateMachineName = "Weapon";
            reuDef.baseMaxStock = 1;
            reuDef.baseRechargeInterval = 4f;
            reuDef.beginSkillCooldownOnSkillEnd = true;
            reuDef.canceledFromSprinting = false;
            reuDef.forceSprintDuringState = false;
            reuDef.dontAllowPastMaxStocks = true;
            reuDef.fullRestockOnAssign = true;
            reuDef.icon = AncientScepter.Assets.SpriteAssets.Bandit2SkullRevolver2;
            reuDef.interruptPriority = InterruptPriority.Skill;
            reuDef.isCombatSkill = true;
            reuDef.keywordTokens = new string[] { "KEYWORD_SLAYER" };
            reuDef.mustKeyPress = false;
            reuDef.cancelSprintingOnActivation = true;
            reuDef.rechargeStock = 1;
            reuDef.requiredStock = 1;
            reuDef.skillName = "RackEmUpScepter";
            reuDef.skillNameToken = "BANDIT2_SPECIAL_ALT_SCEPTER_NAME_RISKYMOD";
            reuDef.skillDescriptionToken = "BANDIT2_SPECIAL_ALT_SCEPTER_DESCRIPTION_RISKYMOD";
            reuDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(reuDef);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(reuDef, "Bandit2Body", SkillSlot.Special, 1);
        }

        private BuffDef BuildSpecialDebuff()
        {
            BuffDef buff = ScriptableObject.CreateInstance<BuffDef>();
            buff.buffColor = new Color(0.8039216f, 0.482352942f, 0.843137264f);
            buff.canStack = true;
            buff.isDebuff = false;
            buff.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffBanditSkullIcon");
            buff.name = "RiskyModBanditRevolver";
            BuffAPI.Add(new CustomBuff(buff));
            return buff;
        }

        private void SpecialDamageType(SkillLocator sk)
        {
            RoR2Content.Survivors.Bandit2.bodyPrefab.AddComponent<SpecialDamageController>();
            GenericSkill passive = RoR2Content.Survivors.Bandit2.bodyPrefab.AddComponent<GenericSkill>();

            SkillDef gunslingerDef = ScriptableObject.CreateInstance<SkillDef>();
            gunslingerDef.activationState = new SerializableEntityStateType(typeof(BaseState));
            gunslingerDef.activationStateMachineName = "Weapon";
            gunslingerDef.skillDescriptionToken = "BANDIT2_REVOLVER_DESCRIPTION_RISKYMOD";
            gunslingerDef.skillName = "Gunslinger";
            gunslingerDef.skillNameToken = "BANDIT2_REVOLVER_NAME_RISKYMOD";
            gunslingerDef.icon = Assets.SkillIcons.Bandit2Gunslinger;
            Skills.Gunslinger = gunslingerDef;
            LoadoutAPI.AddSkillDef(Skills.Gunslinger);

            SkillDef desperado = ScriptableObject.CreateInstance<SkillDef>();
            desperado.activationState = new SerializableEntityStateType(typeof(BaseState));
            desperado.activationStateMachineName = "Weapon";
            desperado.skillDescriptionToken = DesperadoRework.enabled ? "BANDIT2_REVOLVER_ALT_PERSIST_DESCRIPTION_RISKYMOD" : "BANDIT2_REVOLVER_ALT_DESCRIPTION_RISKYMOD";
            desperado.skillName = "Desperado";
            desperado.skillNameToken = "BANDIT2_SPECIAL_ALT_NAME";
            desperado.icon = Assets.SkillIcons.Bandit2Desperado;
            Skills.Desperado = desperado;
            LoadoutAPI.AddSkillDef(Skills.Desperado);

            SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            skillFamily.variants = new SkillFamily.Variant[2];
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = Skills.Gunslinger,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Skills.Gunslinger.skillName, false, null)
            };
            skillFamily.variants[1] = new SkillFamily.Variant
            {
                skillDef = Skills.Desperado,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Skills.Desperado.skillName, false, null)
            };
            LoadoutAPI.AddSkillFamily(skillFamily);
            passive._skillFamily = skillFamily;
        }
        private AnimationCurve BuildSlashVelocityCurve()
        {
            Keyframe kf1 = new Keyframe(0f, 3f, -8.182907104492188f, -3.3333332538604738f, 0f, 0.058712735772132876f);
            kf1.weightedMode = WeightedMode.None;
            kf1.tangentMode = 65;

            Keyframe kf2 = new Keyframe(0.3f, 0f, -3.3333332538604738f, -3.3333332538604738f, 0.3333333432674408f, 0.3333333432674408f);    //Time should match up with SlashBlade min duration (hitbox length)
            kf2.weightedMode = WeightedMode.None;
            kf2.tangentMode = 34;

            Keyframe[] keyframes = new Keyframe[2];
            keyframes[0] = kf1;
            keyframes[1] = kf2;

            return new AnimationCurve
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = keyframes
            };
        }
    }

    public class Skills
    {
        public static SkillDef Gunslinger;
        public static SkillDef Desperado;
    }
}
