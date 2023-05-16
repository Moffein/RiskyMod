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
        public static bool harpoonRangeTweak = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/EngiBody");
        public EngiCore()
        {
            if (!enabled) return;

            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            new PressureMines();
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (harpoonRangeTweak)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Engi.EngiMissilePainter.Paint", "maxDistance", "2000");
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            new TurretChanges();
        }
    }
}
