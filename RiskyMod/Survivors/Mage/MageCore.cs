using RoR2;
using UnityEngine;
using System;
using EntityStates;
using R2API;
using System.Runtime.CompilerServices;
using RoR2.Skills;

namespace RiskyMod.Survivors.Mage
{
    public class MageCore
    {
        public static bool enabled = true;
        public MageCore()
        {
            if (!enabled) return;
            ModifySkills(RoR2Content.Survivors.Mage.bodyPrefab.GetComponent<SkillLocator>());
        }
        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            new M1Projectiles();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Mage.Weapon.Flamethrower");
            for (int i = 0; i < sk.special.skillFamily.variants.Length; i++)
            {
                if (sk.special.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.Weapon.Flamethrower))
                {
                    EntityStates.RiskyMod.Mage.Flamethrower.flamethrowerEffectPrefab
                        = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Mage.Weapon.Flamethrower", "flamethrowerEffectPrefab");
                    sk.special.skillFamily.variants[i].skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.Mage.Flamethrower));
                    sk.special.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_SPECIAL_FIRE_DESCRIPTION_RISKYMOD";

                    LoadoutAPI.AddSkill(typeof(EntityStates.RiskyMod.Mage.Flamethrower));
                    if (RiskyMod.ScepterPluginLoaded)
                    {
                        SetupFlamethrowerScepter(sk, i);
                    }
                }
                else if (sk.special.skillFamily.variants[i].skillDef.activationState.stateType == typeof(EntityStates.Mage.FlyUpState))
                {
                    sk.special.skillFamily.variants[i].skillDef.skillDescriptionToken = "MAGE_SPECIAL_LIGHTNING_DESCRIPTION_RISKYMOD";

                    string keyword = Tweaks.Shock.enabled ? "KEYWORD_SHOCKING_RISKYMOD" : "KEYWORD_SHOCKING";

                    sk.special.skillFamily.variants[i].skillDef.keywordTokens = new string[] { keyword };
                    new IonSurgeTweaks();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupFlamethrowerScepter(SkillLocator sk, int slot)
        {
            SkillDef orig = sk.special.skillFamily.variants[slot].skillDef;
            ScepterHandler.InitFlamethrowerScepter();

            LoadoutAPI.AddSkill(typeof(EntityStates.RiskyMod.Mage.FlamethrowerScepter));
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
            LoadoutAPI.AddSkillDef(skillDef);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(skillDef, "MageBody", SkillSlot.Special, slot);
        }
    }
}
