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
        public static GameObject bodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion();
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
            new BubbleDefenseMatrix();
            if (harpoonRangeTweak)
            {
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Engi/EntityStates.Engi.EngiMissilePainter.Paint.asset", "maxDistance", "2000");
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            new TurretChanges();
        }
    }
}
