using UnityEngine;
using RoR2;
using BepInEx.Configuration;

namespace RiskyMod.Survivors.Merc
{
    public class MercCore
    {
        public static bool enabled = true;

        public static bool modifyStats = true;
        public static ConfigEntry<bool> m1ComboFinishTweak;

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
            On.EntityStates.Merc.Weapon.GroundLight2.OnEnter += (orig, self) =>
            {
                if (m1ComboFinishTweak.Value)
                {
                    if (self.isComboFinisher)
                    {
                        self.ignoreAttackSpeed = true;
                    }
                    else
                    {
                        self.ignoreAttackSpeed = false;
                    }
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
