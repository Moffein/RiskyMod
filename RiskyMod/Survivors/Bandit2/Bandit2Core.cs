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
using static RiskyMod.Content.Assets;

namespace RiskyMod.Survivors.Bandit2
{
    public class Bandit2Core
    {
        public static BuffDef SpecialDebuff;
        public static DamageAPI.ModdedDamageType SpecialDamage;
        public static DamageAPI.ModdedDamageType RackEmUpDamage;
        public static DamageAPI.ModdedDamageType StandoffDamage;

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

        public static BodyIndex Bandit2Index;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/Bandit2Body");

        //Cursed. 5 levels of Standoff, use buff stacks to show duration remaining instead of making a new HUD element.
        public static class Buffs
        {
            public static BuffDef Standoff1, Standoff2, Standoff3, Standoff4, Standoff5;
        }

        public Bandit2Core()
        {
            if (!enabled) return;
            new PersistentDesperado();

            ModifyStats(bodyPrefab.GetComponent<CharacterBody>());
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());

            RoR2Application.onLoad += OnLoad;
        }

        private void OnLoad()
        {
            Bandit2Index = BodyCatalog.FindBodyIndex("Bandit2Body");
        }

        private void ModifyStats(CharacterBody cb)
        {
            if (!modifyStats) return;
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
                Content.Content.entityStates.Add(typeof(FirePrimaryRifle));

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
                SkillDef knifeSkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/SlashBlade.asset").WaitForCompletion();
                if (noKnifeCancel) knifeSkillDef.activationStateMachineName = "KnifeArm";
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

                            //Fire 2 projectiles. Would be cleaner to actually modify the projectile to deal damage twice.
                            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                        }
                    }
                };
            }
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
                cloakDef.interruptPriority = InterruptPriority.Skill;
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
            StandoffDamage = DamageAPI.ReserveDamageType();
            SpecialDamageType(sk);
            SpecialDebuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyModBanditRevolver",
                true,
                false,
                true,   //Just set this to true so people aren't confused.
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
            resetRevolverDef.interruptPriority = InterruptPriority.Skill;
            Skills.LightsOut = resetRevolverDef;

            SkillDef skullRevolverDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/SkullRevolver.asset").WaitForCompletion();
            skullRevolverDef.activationState = new SerializableEntityStateType(typeof(PrepRackEmUp));
            skullRevolverDef.beginSkillCooldownOnSkillEnd = true;
            skullRevolverDef.canceledFromSprinting = false;
            skullRevolverDef.mustKeyPress = false;
            skullRevolverDef.skillNameToken = "BANDIT2_SPECIAL_ALT_NAME_RISKYMOD";
            skullRevolverDef.skillDescriptionToken = "BANDIT2_SPECIAL_ALT_DESCRIPTION_RISKYMOD";
            skullRevolverDef.interruptPriority = InterruptPriority.Skill;
            Skills.RackEmUp = skullRevolverDef;

            if (SoftDependencies.ScepterPluginLoaded)
            {
                BuildScepterSkillDefs(sk);
                SetupScepter();
            }
        }

        public static void ApplyStandoff(CharacterBody body)
        {
            int currentStandoffLevel = 0;
            if (body.HasBuff(Bandit2Core.Buffs.Standoff5))
            {
                currentStandoffLevel = 5;
            }
            else if (body.HasBuff(Bandit2Core.Buffs.Standoff4))
            {
                currentStandoffLevel = 4;
            }
            else if (body.HasBuff(Bandit2Core.Buffs.Standoff3))
            {
                currentStandoffLevel = 3;
            }
            else if (body.HasBuff(Bandit2Core.Buffs.Standoff2))
            {
                currentStandoffLevel = 2;
            }
            else if (body.HasBuff(Bandit2Core.Buffs.Standoff1))
            {
                currentStandoffLevel = 1;
            }

            body.ClearTimedBuffs(Bandit2Core.Buffs.Standoff1);
            body.ClearTimedBuffs(Bandit2Core.Buffs.Standoff2);
            body.ClearTimedBuffs(Bandit2Core.Buffs.Standoff3);
            body.ClearTimedBuffs(Bandit2Core.Buffs.Standoff4);
            body.ClearTimedBuffs(Bandit2Core.Buffs.Standoff5);

            if (currentStandoffLevel < 5) currentStandoffLevel++;
            BuffDef toAdd = null;
            switch (currentStandoffLevel)
            {
                case 5:
                    toAdd = Bandit2Core.Buffs.Standoff5;
                    break;
                case 4:
                    toAdd = Bandit2Core.Buffs.Standoff4;
                    break;
                case 3:
                    toAdd = Bandit2Core.Buffs.Standoff3;
                    break;
                case 2:
                    toAdd = Bandit2Core.Buffs.Standoff2;
                    break;
                case 1:
                    toAdd = Bandit2Core.Buffs.Standoff1;
                    break;
                default:
                    break;
            }
            if (toAdd) SneedUtils.SneedUtils.AddCooldownBuff(body, toAdd, 15);
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
            lightsOutDef.icon = Content.Assets.ScepterSkillIcons.LightsOutScepter;
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
            reuDef.icon = Content.Assets.ScepterSkillIcons.RackEmUpScepter;
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
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.LightsOutScepter, "Bandit2Body", Skills.LightsOut);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.RackEmUpScepter, "Bandit2Body", Skills.RackEmUp);
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
            gunslingerDef.icon = Content.Assets.SkillIcons.Bandit2Gunslinger;
            Skills.Gunslinger = gunslingerDef;
            SneedUtils.SneedUtils.FixSkillName(gunslingerDef);
            Content.Content.skillDefs.Add(Skills.Gunslinger);

            SkillDef desperadoKillStack = ScriptableObject.CreateInstance<SkillDef>();
            desperadoKillStack.activationState = new SerializableEntityStateType(typeof(BaseState));
            desperadoKillStack.activationStateMachineName = "Weapon";
            desperadoKillStack.skillDescriptionToken = PersistentDesperado.enabled ? "BANDIT2_REVOLVER_ALT_PERSIST_DESCRIPTION_RISKYMOD" : "BANDIT2_REVOLVER_ALT_DESCRIPTION_RISKYMOD";
            desperadoKillStack.skillName = "DesperadoKillStack";
            desperadoKillStack.skillNameToken = "BANDIT2_SPECIAL_ALT_NAME";
            desperadoKillStack.icon = Content.Assets.SkillIcons.Bandit2Desperado;
            Skills.DesperadoKillStack = desperadoKillStack;
            SneedUtils.SneedUtils.FixSkillName(desperadoKillStack);
            Content.Content.skillDefs.Add(Skills.DesperadoKillStack);

            SkillDef standoffDef = ScriptableObject.CreateInstance<SkillDef>();
            standoffDef.activationState = new SerializableEntityStateType(typeof(BaseState));
            standoffDef.activationStateMachineName = "Weapon";
            standoffDef.skillDescriptionToken = "BANDIT2_REVOLVER_STANDOFF_DESCRIPTION_RISKYMOD";
            standoffDef.skillName = "Standoff";
            standoffDef.skillNameToken = "BANDIT2_REVOLVER_STANDOFF_NAME_RISKYMOD";
            standoffDef.keywordTokens = new string[] { };
            standoffDef.icon = SkillIcons.Bandit2Standoff;
            Skills.Standoff = standoffDef;
            SneedUtils.SneedUtils.FixSkillName(standoffDef);
            Content.Content.skillDefs.Add(Skills.Standoff);

            BuildStandoff();

            SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            skillFamily.variants = new SkillFamily.Variant[1];

            //Add Gunslinger
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = Skills.Gunslinger,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Skills.Gunslinger.skillName, false, null)
            };

            //Add Desperado
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Skills.DesperadoKillStack,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Skills.DesperadoKillStack.skillName, false, null)
            };

            //Add Standoff
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Skills.Standoff,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Skills.Standoff.skillName, false, null)
            };

            Content.Content.skillFamilies.Add(skillFamily);
            passive._skillFamily = skillFamily;
        }

        private void BuildStandoff()
        {
            #region define buffs
            Buffs.Standoff1 = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_StandoffBuff1",
                true,
                false,
                false,
                Color.white,
                 Content.Assets.BuffIcons.Standoff1
                );

            Buffs.Standoff2 = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_StandoffBuff2",
                true,
                false,
                false,
                Color.white,
                 Content.Assets.BuffIcons.Standoff2
                );

            Buffs.Standoff3 = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_StandoffBuff3",
                true,
                false,
                false,
                Color.white,
                 Content.Assets.BuffIcons.Standoff3
                );

            Buffs.Standoff4 = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_StandoffBuff4",
                true,
                false,
                false,
                Color.white,
                 Content.Assets.BuffIcons.Standoff4
                );
            
            Buffs.Standoff5 = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_StandoffBuff5",
                true,
                false,
                false,
                Color.white,
                 Content.Assets.BuffIcons.Standoff5
                );
            #endregion

            RecalculateStatsAPI.GetStatCoefficients += StandoffBuffStats;

            //On Kill is handled in SpecialDamageTweaks
        }

        private void StandoffBuffStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            float standoffMult = 0f;
            if (sender.HasBuff(Buffs.Standoff5))
            {
                standoffMult = 5f;
            }
            else if (sender.HasBuff(Buffs.Standoff4))
            {
                standoffMult = 4f;
            }
            else if (sender.HasBuff(Buffs.Standoff3))
            {
                standoffMult = 3f;
            }
            else if (sender.HasBuff(Buffs.Standoff2))
            {
                standoffMult = 2f;
            }
            else if (sender.HasBuff(Buffs.Standoff1))
            {
                standoffMult = 1f;
            }
            args.damageMultAdd += standoffMult * 0.2f;
        }
    }

    public class Skills
    {
        public static SkillDef Gunslinger;
        public static SkillDef DesperadoKillStack;
        public static SkillDef Standoff;

        public static SkillDef LightsOut;
        public static SkillDef RackEmUp;

        public static SkillDef LightsOutScepter;
        public static SkillDef RackEmUpScepter;
    }
}
