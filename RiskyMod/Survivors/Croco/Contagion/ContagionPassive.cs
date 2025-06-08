using R2API;
using RiskyMod.Survivors.Croco.Contagion.Components;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco.Contagion
{
    public class ContagionPassive
    {
        public static bool enabled = true;

        public static SkillDef passiveSkillDef;
        public static DamageAPI.ModdedDamageType contagionDamageType;

        private static SkillFamily passiveSkillFamily = Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Croco/CrocoBodyPassiveFamily.asset").WaitForCompletion();
        public static BodyIndex bodyIndex;

        public ContagionPassive()
        {
            if (!enabled) return;
            contagionDamageType = DamageAPI.ReserveDamageType();
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            RoR2Application.onLoadFinished += OnLoad;
            CreateSkillDef();
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CrocoDamageTypeController.GetDamageType += CrocoDamageTypeController_GetDamageType;
            On.EntityStates.Croco.Slash.OnEnter += Slash_OnEnter;
            new ModifySpecial();
            GlobalContagionTracker.Init();
        }

        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.damageType.HasModdedDamageType(contagionDamageType) && damageInfo.damageType.IsDamageSourceSkillBased)
            {
                switch (damageInfo.damageType.damageSource)
                {
                    case DamageSource.Primary:
                        damageInfo.AddModdedDamageType(SharedDamageTypes.CrocoPoison6s);
                        break;
                    case DamageSource.Secondary:
                    case DamageSource.Utility:
                        damageInfo.AddModdedDamageType(SharedDamageTypes.CrocoBlight6s);
                        break;
                    case DamageSource.Special:
                        damageInfo.AddModdedDamageType(ModifySpecial.EpidemicDamage);
                        break;
                    default:
                        break;
                }
            }
            orig(self, damageInfo);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.bodyIndex != bodyIndex || !sender.skillLocator || !HasPassive(sender.skillLocator)) return;
            float levelFactor = sender.level - 1f;
            args.baseDamageAdd -= 3f + (levelFactor * 0.6f);
        }

        private void OnLoad()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("CrocoBody");
        }

        private void Slash_OnEnter(On.EntityStates.Croco.Slash.orig_OnEnter orig, EntityStates.Croco.Slash self)
        {
            orig(self);
            if (self.isComboFinisher && self.skillLocator)
            {
                if (self.overlapAttack != null && HasPassive(self.skillLocator))
                {
                    self.overlapAttack.AddModdedDamageType(contagionDamageType);
                }
            }
        }

        private void CreateSkillDef()
        {
            passiveSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            passiveSkillDef.skillName = "RiskyModCrocoPassiveContagion";
            passiveSkillDef.interruptPriority = EntityStates.InterruptPriority.Any;
            passiveSkillDef.icon = Content.Assets.assetBundle.LoadAsset<Sprite>("AcridRiskyPassive.png");
            passiveSkillDef.skillNameToken = "CROCO_PASSIVE_CONTAGION_NAME_RISKYMOD";
            passiveSkillDef.skillDescriptionToken = "CROCO_PASSIVE_CONTAGION_DESCRIPTION_RISKYMOD";
            passiveSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_CONTAGION_PRIMARY_RISKYMOD",
                "KEYWORD_CONTAGION_SECONDARY_RISKYMOD",
                "KEYWORD_CONTAGION_UTILITY_RISKYMOD",
                "KEYWORD_CONTAGION_SPECIAL_RISKYMOD"
            };
            (passiveSkillDef as ScriptableObject).name = passiveSkillDef.skillName;
            Content.Content.skillDefs.Add(passiveSkillDef);
            SneedUtils.SneedUtils.AddSkillToFamily(passiveSkillFamily, passiveSkillDef);
        }

        //Default to Blighted, manually override things that need overriding.
        private DamageTypeCombo CrocoDamageTypeController_GetDamageType(On.RoR2.CrocoDamageTypeController.orig_GetDamageType orig, CrocoDamageTypeController self)
        {
            if (self.passiveSkillSlot && self.passiveSkillSlot.skillDef == passiveSkillDef)
            {
                DamageTypeCombo combo = DamageTypeCombo.Generic;
                combo.AddModdedDamageType(contagionDamageType);
                return combo;
            }
            return orig(self);
        }

        public static bool HasPassive(SkillLocator skillLocator)
        {
            if (skillLocator)
            {
                foreach (GenericSkill skill in skillLocator.allSkills)
                {
                    if (skill.skillFamily == passiveSkillFamily)
                    {
                        return skill.skillDef == passiveSkillDef;
                    }
                }
            }
            return false;
        }
    }
}
