using UnityEngine;
using RoR2;

namespace RiskyMod.Survivors.Merc
{
    public class MercCore
    {
        public static bool enabled = true;

        public static bool modifyStats = true;
        public static bool m1ComboFinishTweak = true;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MercBody");
        public MercCore()
        {
            if (!enabled) return;
            ModifyStats(bodyPrefab.GetComponent<CharacterBody>());
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
        }
        private void ModifyPrimaries(SkillLocator sk)
        {
            if (!m1ComboFinishTweak) return;

            On.EntityStates.Merc.Weapon.GroundLight2.OnEnter += (orig, self) =>
            {
                if (self.isComboFinisher)
                {
                    self.ignoreAttackSpeed = true;
                }
                else
                {
                    self.ignoreAttackSpeed = false;
                }
                 orig(self);
            };
        }

        private void ModifyStats(CharacterBody cb)
        {
            if (!modifyStats) return;
            cb.baseRegen = 2.5f;
            cb.levelRegen = cb.baseRegen * 0.2f;
        }
    }
}
