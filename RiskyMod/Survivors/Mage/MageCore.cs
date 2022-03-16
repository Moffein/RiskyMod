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

namespace RiskyMod.Survivors.Mage
{
    //TODO: ADD CONFIG FOR NEW M1 CHANGES
    public class MageCore
    {
        public static bool enabled = true;

        public static bool m1AttackSpeed = true;
        public static bool modifyFireBolt = true;
        public static bool modifyPlasmaBolt = true;

        public static bool flamethrowerSprintCancel = true;

        public static bool ionSurgeShock = true;
        public static bool ionSurgeMovementScaling = false;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MageBody");

        public MageCore()
        {
            if (!enabled) return;
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }
        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
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
                        SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.FireFireBolt", "damageCoefficient", "3");
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
            new M1Projectiles();
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            //new SolidIceWall();
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
                else if (sk.special.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.FlyUpState))
                {
                    if (ionSurgeShock)
                    {
                        sk.special.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_SPECIAL_LIGHTNING_DESCRIPTION_RISKYMOD";
                        string keyword = Tweaks.Shock.enabled ? "KEYWORD_SHOCKING_RISKYMOD" : "KEYWORD_SHOCKING";
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
}
