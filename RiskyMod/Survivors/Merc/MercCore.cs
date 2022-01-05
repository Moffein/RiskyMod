using UnityEngine;
using RoR2;

namespace RiskyMod.Survivors.Merc
{
    public class MercCore
    {
        public static bool enabled = true;
        public MercCore()
        {
            if (!enabled) return;
            ModifyStats(RoR2Content.Survivors.Merc.bodyPrefab.GetComponent<CharacterBody>());
        }

        private void ModifyStats(CharacterBody cb)
        {
            cb.baseRegen = 2.5f;
            cb.levelRegen = cb.baseRegen * 0.2f;
        }
    }
}
