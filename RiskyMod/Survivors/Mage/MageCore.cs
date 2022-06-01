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

namespace RiskyMod.Survivors.Mage
{
    public class MageCore
    {
        public static bool enabled = true;

        public static bool m1AttackSpeed = true;
        public static bool modifyFireBolt = true;
        public static bool modifyPlasmaBolt = true;

        public static bool m2RemoveNanobombGravity = true;

        public static bool flamethrowerSprintCancel = true;
        public static bool flamethrowerRangeExtend = true;

        public static bool ionSurgeShock = true;
        public static bool ionSurgeMovementScaling = false;

        public static bool ionSurgeUtility = true;

        public static bool enableFireUtility = true;
        public static bool enableLightningSpecial = false;

        public static bool iceWallRework = true;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MageBody");

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
            //Range increase always gets run now
            new M1Projectiles();

            if (m1AttackSpeed)
            {
                MageStockController.StatePairs.Add(typeof(EntityStates.Mage.Weapon.FireFireBolt), MageStockController.fireMuzzleflashEffectPrefab);
                MageStockController.StatePairs.Add(typeof(EntityStates.Mage.Weapon.FireLightningBolt), MageStockController.lightningMuzzleflashEffectPrefab);
                bodyPrefab.AddComponent<MageStockController>();
            }

            for (int i = 0; i < sk.primary.skillFamily.variants.Length; i++)
            {
                if (sk.primary.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.FireFireBolt))
                {
                    if (m1AttackSpeed)
                    {
                        sk.primary.skillFamily.variants[i].skillDef.rechargeStock = 0;
                        //sk.primary.skillFamily.variants[i].skillDef.baseRechargeInterval = 0;

                        //FireLightningBolt inherits from this
                        On.EntityStates.Mage.Weapon.FireFireBolt.OnEnter += (orig, self) =>
                        {
                            orig(self);
                            MageStockController msc = self.gameObject.GetComponent<MageStockController>();
                            if (msc)
                            {
                                msc.FireSkill(self.duration);
                            }
                        };
                    }

                    if (modifyFireBolt)
                    {
                        sk.primary.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_PRIMARY_FIRE_DESCRIPTION_RISKYMOD";
                        M1Projectiles.ModifyFireBolt();
                    }
                }
                else if (sk.primary.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.FireLightningBolt))
                {
                    if (m1AttackSpeed)
                    {
                        sk.primary.skillFamily.variants[i].skillDef.rechargeStock = 0;
                        //sk.primary.skillFamily.variants[i].skillDef.baseRechargeInterval = 0;
                    }

                    if (modifyPlasmaBolt)
                    {
                        sk.primary.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_PRIMARY_LIGHTNING_DESCRIPTION_RISKYMOD";
                        M1Projectiles.ModifyLightningBolt();
                    }
                }
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

                SkillDef iceSkill = ScriptableObject.CreateInstance<SkillDef>();

                iceSkill.activationState = new SerializableEntityStateType(typeof(PrepIceWall));
                iceSkill.activationStateMachineName = "Weapon";
                iceSkill.baseMaxStock = 1;
                iceSkill.baseRechargeInterval = 12f;
                iceSkill.beginSkillCooldownOnSkillEnd = true;
                iceSkill.canceledFromSprinting = !RiskyMod.RtAutoSprintLoaded;
                iceSkill.cancelSprintingOnActivation = true;
                iceSkill.dontAllowPastMaxStocks = true;
                iceSkill.forceSprintDuringState = false;
                iceSkill.fullRestockOnAssign = true;
                iceSkill.icon = sk.utility.skillFamily.variants[0].skillDef.icon;
                iceSkill.interruptPriority = InterruptPriority.PrioritySkill;
                iceSkill.isCombatSkill = true;
                iceSkill.keywordTokens = new string[] { "KEYWORD_FREEZING" };
                iceSkill.mustKeyPress = false;
                iceSkill.rechargeStock = 1;
                iceSkill.requiredStock = 1;
                iceSkill.resetCooldownTimerOnUse = false;
                iceSkill.skillDescriptionToken = "MAGE_UTILITY_ICE_DESCRIPTION_RISKYMOD";
                iceSkill.skillNameToken = "MAGE_UTILITY_ICE_NAME";
                iceSkill.skillName = "RiskyModIceWall";
                iceSkill.stockToConsume = 1;
                SneedUtils.SneedUtils.FixSkillName(iceSkill);

                Skills.PrepIceWall = iceSkill;
                Content.Content.skillDefs.Add(Skills.PrepIceWall);
                sk.utility.skillFamily.variants[0].skillDef = Skills.PrepIceWall;
            }

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
                fireStorm.canceledFromSprinting = !RiskyMod.RtAutoSprintLoaded;
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

                Skills.PrepIonSurge = fireStorm;
                Content.Content.skillDefs.Add(Skills.PrepIonSurge);

                SkillFamily utilitySkillFamily = sk.utility.skillFamily;
                Array.Resize(ref utilitySkillFamily.variants, utilitySkillFamily.variants.Length + 1);
                utilitySkillFamily.variants[utilitySkillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = fireStorm,
                    viewableNode = new ViewablesCatalog.Node(fireStorm.skillNameToken, false)
                };

                GameObject fireStormProjectile = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireTornado").InstantiateClone("RiskyMod_MageFireStorm", true);

                ProjectileSimple ps = fireStormProjectile.GetComponent<ProjectileSimple>();
                //ps.lifetime = 5f; //Particle doesn't scale to lifetime

                ProjectileOverlapAttack poa = fireStormProjectile.GetComponent<ProjectileOverlapAttack>();
                poa.overlapProcCoefficient = 1f;
                //poa.resetInterval = 0.33f;
                poa.damageCoefficient = poa.resetInterval / ps.lifetime;

                ProjectileDamage pd = fireStormProjectile.GetComponent<ProjectileDamage>();
                pd.damageType = DamageType.IgniteOnHit;

                Content.Content.projectilePrefabs.Add(fireStormProjectile);
                EntityStates.RiskyMod.Mage.Weapon.PrepFireStorm.projectilePrefab = fireStormProjectile;
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Mage.Weapon.Flamethrower");
            for (int i = 0; i < sk.special.skillFamily.variants.Length; i++)
            {
                if (sk.special.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.Flamethrower))
                {
                    sk.special.skillFamily.variants[i].skillDef.canceledFromSprinting = flamethrowerSprintCancel;
                    if (flamethrowerRangeExtend)
                    {
                        SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.Flamethrower", "maxDistance", "25");  //20 vanilla
                    }
                }
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
                skillDef.canceledFromSprinting = flamethrowerSprintCancel;
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
            }
        }

        private void HandleIonSurge(SkillLocator sk)
        {
            SkillDef surgeDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlyUp.asset").WaitForCompletion();
            if (ionSurgeShock)
            {
                surgeDef.skillDescriptionToken = "MAGE_SPECIAL_LIGHTNING_DESCRIPTION_RISKYMOD";
                string keyword = Tweaks.CharacterMechanics.Shock.enabled ? "KEYWORD_SHOCKING_RISKYMOD" : "KEYWORD_SHOCKING";
                surgeDef.keywordTokens = new string[] { keyword };

                IL.EntityStates.Mage.FlyUpState.OnEnter += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchCallvirt<BlastAttack>("Fire")
                        );
                    c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                    {
                        blastAttack.damageType = DamageType.Shock5s;
                        return blastAttack;
                    });
                };

            }

            if (!ionSurgeMovementScaling)
            {
                IL.EntityStates.Mage.FlyUpState.HandleMovements += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchLdfld<EntityStates.BaseState>("moveSpeedStat")
                        );
                    c.Index++;
                    c.EmitDelegate<Func<float, float>>(orig =>
                    {
                        return 7f;
                    });
                };
            }

            if (ionSurgeUtility)
            {
                //Remove Ion Surge from Specials
                sk.special.skillFamily.variants = sk.special.skillFamily.variants.Where(v => v.skillDef.activationState.stateType != typeof(EntityStates.Mage.FlyUpState)).ToArray();

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
    }

    public static class Skills
    {
        public static SkillDef PrepIceWall;
        public static SkillDef PrepIonSurge;
        public static SkillDef SpecialLightning;
    }
}
