using UnityEngine;
using RoR2;

namespace RiskyMod.Survivors.Merc
{
    public class MercCore
    {
        public static bool enabled = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MercBody");
        public MercCore()
        {
            if (!enabled) return;
            ModifyStats(bodyPrefab.GetComponent<CharacterBody>());
        }

        private void ModifyStats(CharacterBody cb)
        {
            cb.baseRegen = 2.5f;
            cb.levelRegen = cb.baseRegen * 0.2f;
        }
    }
}
