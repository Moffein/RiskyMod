using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

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
            SkillDef mines = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceMine.asset").WaitForCompletion();

            mines.skillDescriptionToken = "ENGI_SECONDARY_DESCRIPTION_RISKYMOD";
            new PressureMines();
        }
        private void ModifySpecials(SkillLocator sk)
        {
            new TurretChanges();
        }
    }
}
