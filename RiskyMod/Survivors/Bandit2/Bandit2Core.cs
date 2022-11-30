using EntityStates;
using EntityStates.RiskyMod.Bandit2;
using EntityStates.RiskyMod.Bandit2.Primary;
using EntityStates.RiskyMod.Bandit2.Revolver;
using EntityStates.RiskyMod.Bandit2.Revolver.Scepter;
using MonoMod.RuntimeDetour;
using R2API;
using R2API.Utils;
using RiskyMod.Content;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Bandit2
{
    public class Bandit2Core
    {
        public static BuffDef SpecialDebuff;
        public static DamageAPI.ModdedDamageType SpecialDamage;
        public static DamageAPI.ModdedDamageType RackEmUpDamage;

        public static DamageAPI.ModdedDamageType RevolverRicochet;

        public static bool enabled = true;

        public static bool modifyStats = true;

        public static bool blastChanges = true;
        public static bool burstChanges = true;

        public static bool knifeChanges = true;
        public static bool knifeThrowChanges = true;
        public static bool noKnifeCancel = true;

        public static bool utilityChanges = true;
        public static bool utilityFix = true;

        public static bool specialRework = true;

        public static bool enableDesperadoKillStack = true;

        public static BodyIndex Bandit2Index;
        private static AnimationCurve knifeVelocity;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/Bandit2Body");

        public Bandit2Core()
        {
            if (!enabled) return;
            new BanditSpecialGracePeriod();
            new DesperadoRework();

            ModifyStats(bodyPrefab.GetComponent<CharacterBody>());
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());

            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                Bandit2Index = BodyCatalog.FindBodyIndex("Bandit2Body");
            };
        }

        private void ModifyStats(CharacterBody cb)
        {
            cb.baseMaxHealth = 100f;//110f
            cb.levelMaxHealth = 30f;//33f
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
            if (BackstabRework.enabled)
            {
                new BackstabRework();
                sk.passiveSkill.skillDescriptionToken = "BANDIT2_PASSIVE_DESCRIPTION_RISKYMOD";
            }
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            Content.Content.entityStates.Add(typeof(EnterReload));
            Content.Content.entityStates.Add(typeof(Reload));

            if (burstChanges)
            {
                Content.Content.entityStates.Add(typeof(FirePrimaryShotgun));

                ReloadSkillDef burstDef = Addressables.LoadAssetAsync<ReloadSkillDef>("RoR2/Base/Bandit2/FireShotgun2.asset").WaitForCompletion();
                burstDef.activationState = new SerializableEntityStateType(typeof(FirePrimaryShotgun));
                burstDef.skillDescriptionToken = "BANDIT2_PRIMARY_DESC_RISKYMOD";
                burstDef.reloadState = new SerializableEntityStateType(typeof(EnterReload));
                burstDef.mustKeyPress = false;
            }

            if (blastChanges)
            {
                ReloadSkillDef blastDef = Addressables.LoadAssetAsync<ReloadSkillDef>("RoR2/Base/Bandit2/Bandit2Blast.asset").WaitForCompletion();
                blastDef.activationState = new SerializableEntityStateType(typeof(FirePrimaryRifle));
                blastDef.skillDescriptionToken = "BANDIT2_PRIMARY_ALT_DESC_RISKYMOD";
                blastDef.reloadState = new SerializableEntityStateType(typeof(EnterReload));
                blastDef.mustKeyPress = false;
            }
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (BuffHemorrhage.enabled) new BuffHemorrhage();

            if (noKnifeCancel)
            {
                EntityStateMachine knifeStateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
                knifeStateMachine.customName = "KnifeArm";
                knifeStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
                knifeStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));

                NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();
                nsm.stateMachines = nsm.stateMachines.Append(knifeStateMachine).ToArray();
            }

            if (knifeChanges)
            {
                new IncreaseKnifeHitboxSize();
                knifeVelocity = BuildSlashVelocityCurve();

                SkillDef knifeSkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/SlashBlade.asset").WaitForCompletion();

                knifeSkillDef.canceledFromSprinting = false;

                if (noKnifeCancel) knifeSkillDef.activationStateMachineName = "KnifeArm";

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
                SkillDef shivSkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/Bandit2SerratedShivs.asset").WaitForCompletion();
                shivSkillDef.keywordTokens = new string[] { "KEYWORD_STUNNING", "KEYWORD_SUPERBLEED" };
                shivSkillDef.skillDescriptionToken = "BANDIT2_SECONDARY_ALT_DESCRIPTION_RISKYMOD";
                if (noKnifeCancel) shivSkillDef.activationStateMachineName = "KnifeArm";

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
            SkillDef cloakDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/ThrowSmokebomb.asset").WaitForCompletion();
            if (utilityFix)
            {
                Content.Content.entityStates.Add(typeof(ThrowSmokebomb));
                Content.Content.entityStates.Add(typeof(StealthMode));
                cloakDef.activationState = new SerializableEntityStateType(typeof(ThrowSmokebomb));
                cloakDef.mustKeyPress = false;
            }
            if (utilityChanges)
            {
                cloakDef.baseRechargeInterval = 7f;
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (!specialRework) return;
            SpecialDamage = DamageAPI.ReserveDamageType();
            RackEmUpDamage = DamageAPI.ReserveDamageType();
            RevolverRicochet = DamageAPI.ReserveDamageType();
            SpecialDamageType(sk);
            SpecialDebuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyModBanditRevolver",
                true,
                false,
                false,
                new Color(0.8039216f, 0.482352942f, 0.843137264f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite
                );
            new SpecialDamageTweaks();

            Content.Content.entityStates.Add(typeof(BaseSidearmState));
            Content.Content.entityStates.Add(typeof(ExitSidearm));
            Content.Content.entityStates.Add(typeof(PrepLightsOut));
            Content.Content.entityStates.Add(typeof(FireLightsOut));

            SkillDef resetRevolverDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/ResetRevolver.asset").WaitForCompletion();
            resetRevolverDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOut));
            resetRevolverDef.beginSkillCooldownOnSkillEnd = true;
            resetRevolverDef.canceledFromSprinting = false;
            resetRevolverDef.skillDescriptionToken = "BANDIT2_SPECIAL_DESCRIPTION_RISKYMOD";
            resetRevolverDef.mustKeyPress = false;
            Skills.LightsOut = resetRevolverDef;

            SkillDef skullRevolverDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/SkullRevolver.asset").WaitForCompletion();
            skullRevolverDef.activationState = new SerializableEntityStateType(typeof(PrepRackEmUp));
            skullRevolverDef.beginSkillCooldownOnSkillEnd = true;
            skullRevolverDef.canceledFromSprinting = false;
            skullRevolverDef.mustKeyPress = false;
            skullRevolverDef.skillNameToken = "BANDIT2_SPECIAL_ALT_NAME_RISKYMOD";
            skullRevolverDef.skillDescriptionToken = "BANDIT2_SPECIAL_ALT_DESCRIPTION_RISKYMOD";
            Skills.RackEmUp = skullRevolverDef;

            if (SoftDependencies.ScepterPluginLoaded || SoftDependencies.ClassicItemsScepterLoaded)
            {
                BuildScepterSkillDefs(sk);
                if (SoftDependencies.ScepterPluginLoaded) SetupScepter();
                if (SoftDependencies.ClassicItemsScepterLoaded) SetupScepterClassic();
            }    
        }

        private void BuildScepterSkillDefs(SkillLocator sk)
        {
            SkillDef lightsOutDef = SkillDef.CreateInstance<SkillDef>();
            Content.Content.entityStates.Add(typeof(PrepLightsOutScepter));
            Content.Content.entityStates.Add(typeof(FireLightsOutScepter));
            lightsOutDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOutScepter));
            lightsOutDef.activationStateMachineName = "Weapon";
            lightsOutDef.baseMaxStock = 1;
            lightsOutDef.baseRechargeInterval = 4f;
            lightsOutDef.beginSkillCooldownOnSkillEnd = true;
            lightsOutDef.canceledFromSprinting = false;
            lightsOutDef.forceSprintDuringState = false;
            lightsOutDef.dontAllowPastMaxStocks = true;
            lightsOutDef.fullRestockOnAssign = true;
            lightsOutDef.icon = Assets.ScepterSkillIcons.LightsOutScepter;
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
            SneedUtils.SneedUtils.FixSkillName(lightsOutDef);
            Content.Content.skillDefs.Add(lightsOutDef);

            Skills.LightsOutScepter = lightsOutDef;

            SkillDef reuDef = SkillDef.CreateInstance<SkillDef>();
            Content.Content.entityStates.Add(typeof(PrepRackEmUpScepter));
            Content.Content.entityStates.Add(typeof(FireRackEmUpScepter));
            reuDef.activationState = new SerializableEntityStateType(typeof(PrepRackEmUpScepter));
            reuDef.activationStateMachineName = "Weapon";
            reuDef.baseMaxStock = 1;
            reuDef.baseRechargeInterval = 4f;
            reuDef.beginSkillCooldownOnSkillEnd = true;
            reuDef.canceledFromSprinting = false;
            reuDef.forceSprintDuringState = false;
            reuDef.dontAllowPastMaxStocks = true;
            reuDef.fullRestockOnAssign = true;
            reuDef.icon = Assets.ScepterSkillIcons.RackEmUpScepter;
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
            SneedUtils.SneedUtils.FixSkillName(reuDef);
            Content.Content.skillDefs.Add(reuDef);

            Skills.RackEmUpScepter = reuDef;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.LightsOutScepter, "Bandit2Body", SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.RackEmUpScepter, "Bandit2Body", SkillSlot.Special, 1);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepterClassic()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.LightsOutScepter, "Bandit2Body", SkillSlot.Special, Skills.LightsOut);
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.RackEmUpScepter, "Bandit2Body", SkillSlot.Special, Skills.RackEmUp);
        }

        private void SpecialDamageType(SkillLocator sk)
        {
            bodyPrefab.AddComponent<SpecialDamageController>();
            GenericSkill passive = bodyPrefab.AddComponent<GenericSkill>();

            SkillDef gunslingerDef = ScriptableObject.CreateInstance<SkillDef>();
            gunslingerDef.activationState = new SerializableEntityStateType(typeof(BaseState));
            gunslingerDef.activationStateMachineName = "Weapon";
            gunslingerDef.skillDescriptionToken = "BANDIT2_REVOLVER_DESCRIPTION_RISKYMOD";
            gunslingerDef.skillName = "Gunslinger";
            gunslingerDef.skillNameToken = "BANDIT2_REVOLVER_NAME_RISKYMOD";
            gunslingerDef.keywordTokens = new string[] {};
            gunslingerDef.icon = Assets.SkillIcons.Bandit2Gunslinger;
            Skills.Gunslinger = gunslingerDef;
            SneedUtils.SneedUtils.FixSkillName(gunslingerDef);
            Content.Content.skillDefs.Add(Skills.Gunslinger);

            SkillDef desperadoKillStack = ScriptableObject.CreateInstance<SkillDef>();
            desperadoKillStack.activationState = new SerializableEntityStateType(typeof(BaseState));
            desperadoKillStack.activationStateMachineName = "Weapon";
            desperadoKillStack.skillDescriptionToken = DesperadoRework.enabled ? "BANDIT2_REVOLVER_ALT_PERSIST_DESCRIPTION_RISKYMOD" : "BANDIT2_REVOLVER_ALT_DESCRIPTION_RISKYMOD";
            desperadoKillStack.skillName = "DesperadoKillStack";
            desperadoKillStack.skillNameToken = "BANDIT2_SPECIAL_ALT_NAME";
            desperadoKillStack.icon = Assets.SkillIcons.Bandit2Desperado;
            Skills.DesperadoKillStack = desperadoKillStack;
            SneedUtils.SneedUtils.FixSkillName(desperadoKillStack);
            Content.Content.skillDefs.Add(Skills.DesperadoKillStack);

            SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            skillFamily.variants = new SkillFamily.Variant[1];
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = Skills.Gunslinger,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Skills.Gunslinger.skillName, false, null)
            };

            if (enableDesperadoKillStack)
            {
                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = Skills.DesperadoKillStack,
                    unlockableDef = null,
                    viewableNode = new ViewablesCatalog.Node(Skills.DesperadoKillStack.skillName, false, null)
                };
            }

            Content.Content.skillFamilies.Add(skillFamily);
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
        public static SkillDef DesperadoKillStack;

        public static SkillDef LightsOut;
        public static SkillDef RackEmUp;

        public static SkillDef LightsOutScepter;
        public static SkillDef RackEmUpScepter;
    }
}
