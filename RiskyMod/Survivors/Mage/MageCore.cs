using RoR2;
using UnityEngine;
using System;
using EntityStates;
using R2API;
using System.Runtime.CompilerServices;
using RoR2.Skills;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Survivors.Mage
{
    //TODO: ADD CONFIG FOR NEW M1 CHANGES
    public class MageCore
    {
        public static bool enabled = true;

        public static bool flamethrowerAttackSpeed = true;
        public static bool flamethrowerSprintCancel = true;
        public static bool m2Buffer = true;
        public static bool m2RequiresKeypress = false;

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
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            for (int i = 0; i < sk.primary.skillFamily.variants.Length; i++)
            {
                if (sk.primary.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.FireFireBolt))
                {

                }
                else if (sk.primary.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.FireLightningBolt))
                {
                    if (M1Projectiles.modifyPlasma)
                    {
                        sk.primary.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_PRIMARY_LIGHTNING_DESCRIPTION_RISKYMOD";
                    }
                }
            }
            //new QuickdrawPassive();
            new M1Projectiles();
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (m2RequiresKeypress)
            {
                for (int i = 0; i < sk.secondary.skillFamily.variants.Length; i++)
                {
                    if (sk.secondary.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.ChargeNovabomb))
                    {
                        sk.secondary.skillFamily.variants[i].skillDef.mustKeyPress = true;
                    }
                    else if (sk.secondary.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.ChargeIcebomb))
                    {
                        sk.secondary.skillFamily.variants[i].skillDef.mustKeyPress = true;
                    }
                }
            }
            if (m2Buffer)
            {
                IL.EntityStates.Mage.Weapon.BaseThrowBombState.FixedUpdate += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(MoveType.After,
                         x => x.MatchLdfld<EntityStates.Mage.Weapon.BaseThrowBombState>("duration")
                        );
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, EntityStates.Mage.Weapon.BaseThrowBombState, float>>((duration, self) =>
                    {
                        if (self.inputBank && self.inputBank.skill2.down)
                        {
                            return 0.6f;
                        }
                        return duration;
                    });
                };
            }
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
                    R2API.ContentAddition.AddEntityState<EntityStates.RiskyMod.Mage.Flamethrower>(out bool x);
                    if (flamethrowerAttackSpeed)
                    {
                        EntityStates.RiskyMod.Mage.Flamethrower.flamethrowerEffectPrefab
                            = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Mage.Weapon.Flamethrower", "flamethrowerEffectPrefab");
                        sk.special.skillFamily.variants[i].skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.Mage.Flamethrower));
                        sk.special.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_SPECIAL_FIRE_DESCRIPTION_RISKYMOD";

                        if (RiskyMod.ScepterPluginLoaded)
                        {
                            SetupFlamethrowerScepter(sk, i);
                        }
                    }
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

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupFlamethrowerScepter(SkillLocator sk, int slot)
        {
            SkillDef orig = sk.special.skillFamily.variants[slot].skillDef;
            ScepterHandler.InitFlamethrowerScepter();

            R2API.ContentAddition.AddEntityState<EntityStates.RiskyMod.Mage.FlamethrowerScepter>(out bool x);
            SkillDef skillDef = SkillDef.CreateInstance<SkillDef>();
            skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.Mage.FlamethrowerScepter));
            skillDef.activationStateMachineName = orig.activationStateMachineName;
            skillDef.baseMaxStock = orig.baseMaxStock;
            skillDef.baseRechargeInterval = orig.baseRechargeInterval;
            skillDef.beginSkillCooldownOnSkillEnd = orig.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = orig.canceledFromSprinting;
            skillDef.dontAllowPastMaxStocks = orig.dontAllowPastMaxStocks;
            skillDef.forceSprintDuringState = orig.forceSprintDuringState;
            skillDef.fullRestockOnAssign = orig.fullRestockOnAssign;
            skillDef.icon = AncientScepter.Assets.SpriteAssets.ArtificerFlameThrower2;
            skillDef.interruptPriority = orig.interruptPriority;
            skillDef.isCombatSkill = orig.isCombatSkill;
            skillDef.keywordTokens = orig.keywordTokens;
            skillDef.mustKeyPress = orig.mustKeyPress;
            skillDef.cancelSprintingOnActivation = orig.cancelSprintingOnActivation;
            skillDef.rechargeStock = orig.rechargeStock;
            skillDef.requiredStock = orig.requiredStock;
            skillDef.skillName = orig.skillName + "Scepter";
            skillDef.skillNameToken = "MAGE_SPECIAL_FIRE_SCEPTER_NAME_RISKYMOD";
            skillDef.skillDescriptionToken = "MAGE_SPECIAL_FIRE_SCEPTER_DESCRIPTION_RISKYMOD";
            skillDef.stockToConsume = 1;
            R2API.ContentAddition.AddSkillDef(skillDef);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(skillDef, "MageBody", SkillSlot.Special, slot);
        }
    }
}
