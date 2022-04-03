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

namespace RiskyMod.Survivors.Mage
{
    //TODO: ADD CONFIG FOR NEW M1 CHANGES
    public class MageCore
    {
        public static bool enabled = true;

        public static bool m1AttackSpeed = true;
        public static bool modifyFireBolt = true;
        public static bool modifyPlasmaBolt = true;

        public static bool m2RemoveNanobombGravity = true;

        public static bool flamethrowerSprintCancel = true;

        public static bool ionSurgeShock = true;
        public static bool ionSurgeMovementScaling = false;

        public static bool ionSurgeRework = true;

        public static bool iceWallRework = true;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MageBody");

        public MageCore()
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
            //Range increase always gets run now
            new M1Projectiles();

            if (m1AttackSpeed)
            {
                MageStockController.fireMuzzleflashEffectPrefab = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Mage.Weapon.FireFireBolt", "muzzleflashEffectPrefab");
                MageStockController.lightningMuzzleflashEffectPrefab = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Mage.Weapon.FireLightningBolt", "muzzleflashEffectPrefab");
                MageStockController.iceMuzzleflashEffectPrefab = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Mage.Weapon.FireIceBolt", "muzzleflashEffectPrefab");
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
                        SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireLightningBolt", "damageCoefficient", "3");
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
                iceSkill.canceledFromSprinting = true;
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

            if (ionSurgeRework)
            {
                //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Mage.FlyUpState"); SkillDef ionSurge2 = ScriptableObject.CreateInstance<SkillDef>();
                Content.Content.entityStates.Add(typeof(PrepIonSurge));

                SkillDef ionSurge2 = ScriptableObject.CreateInstance<SkillDef>();
                ionSurge2.activationState = new SerializableEntityStateType(typeof(PrepIonSurge));
                ionSurge2.activationStateMachineName = "Weapon";
                ionSurge2.baseMaxStock = 1;
                ionSurge2.baseRechargeInterval = 12f;
                ionSurge2.beginSkillCooldownOnSkillEnd = true;
                ionSurge2.canceledFromSprinting = true;
                ionSurge2.cancelSprintingOnActivation = true;
                ionSurge2.dontAllowPastMaxStocks = true;
                ionSurge2.forceSprintDuringState = false;
                ionSurge2.fullRestockOnAssign = true;
                ionSurge2.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlyUp.asset").WaitForCompletion().icon;
                ionSurge2.interruptPriority = InterruptPriority.PrioritySkill;
                ionSurge2.isCombatSkill = true;
                string keyword = Tweaks.CharacterMechanics.Shock.enabled ? "KEYWORD_SHOCKING_RISKYMOD" : "KEYWORD_SHOCKING";
                ionSurge2.keywordTokens = new string[] { keyword };
                ionSurge2.mustKeyPress = false;
                ionSurge2.rechargeStock = 1;
                ionSurge2.requiredStock = 1;
                ionSurge2.resetCooldownTimerOnUse = false;
                ionSurge2.skillDescriptionToken = "MAGE_UTILITY_LIGHTNING_DESCRIPTION_RISKYMOD";
                ionSurge2.skillNameToken = "MAGE_SPECIAL_LIGHTNING_NAME";
                ionSurge2.skillName = "RiskyModIonSurge";
                ionSurge2.stockToConsume = 1;
                SneedUtils.SneedUtils.FixSkillName(ionSurge2);

                Skills.PrepIonSurge = ionSurge2;
                Content.Content.skillDefs.Add(Skills.PrepIonSurge);

                SkillFamily secondarySkillFamily = sk.utility.skillFamily;
                Array.Resize(ref secondarySkillFamily.variants, secondarySkillFamily.variants.Length + 1);
                secondarySkillFamily.variants[secondarySkillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = ionSurge2,
                    unlockableDef = Addressables.LoadAssetAsync<UnlockableDef>("RoR2/Base/Mage/Skills.Mage.FlyUp.asset").WaitForCompletion(),
                    viewableNode = new ViewablesCatalog.Node(ionSurge2.skillNameToken, false)
                };
                
                //Remove Vanilla Ion Surge
                sk.special.skillFamily.variants = sk.special.skillFamily.variants.Where(v => v.skillDef.activationState.stateType != typeof(EntityStates.Mage.FlyUpState)).ToArray();
                //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Toolbot.AimStunDrone");
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
                }
                else if (sk.special.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.FlyUpState) && !ionSurgeRework)
                {
                    if (ionSurgeShock)
                    {
                        sk.special.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_SPECIAL_LIGHTNING_DESCRIPTION_RISKYMOD";
                        string keyword = Tweaks.CharacterMechanics.Shock.enabled ? "KEYWORD_SHOCKING_RISKYMOD" : "KEYWORD_SHOCKING";
                        sk.special.skillFamily.variants[i].skillDef.keywordTokens = new string[] { keyword };

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
                }
            }
        }
    }

    public static class Skills
    {
        public static SkillDef PrepIceWall;
        public static SkillDef PrepIonSurge;
    }
}
