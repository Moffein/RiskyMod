using RoR2;
using UnityEngine;
using R2API;

namespace RiskyMod.Survivors.Engi
{
    public class EngiCore
    {
        public static bool enabled = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/EngiBody");
        public EngiCore()
        {
            if (!enabled) return;

            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifySecondaries(sk);
            ModifySpecials(sk);
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "ENGI_SECONDARY_DESCRIPTION_RISKYMOD";
            new PressureMines();
        }
        private void ModifySpecials(SkillLocator sk)
        {
            new TurretChanges();
        }
    }
}
