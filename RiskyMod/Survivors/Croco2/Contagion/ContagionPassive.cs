using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco2.Contagion
{
    public class ContagionPassive
    {
        public static bool enabled = true;

        public static SkillDef passiveSkillDef;
        public ContagionPassive()
        {
            if (!enabled) return;

            CreateSkillDef();
        }

        private void CreateSkillDef()
        {
            passiveSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            passiveSkillDef.skillName = "RiskyModCrocoPassiveContagion";
            passiveSkillDef.interruptPriority = EntityStates.InterruptPriority.Any;
            passiveSkillDef.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoPassiveBlight.asset").WaitForCompletion().icon;
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
            SneedUtils.SneedUtils.AddSkillToFamily(Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Croco/CrocoBodyPassiveFamily.asset").WaitForCompletion(), passiveSkillDef);
        }
    }
}
