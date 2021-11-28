using UnityEngine;
using RoR2;

namespace RiskyMod.Survivors.Bandit2
{
    public class IncreaseKnifeHitboxSize
    {
        public static bool enabled = true;
        public IncreaseKnifeHitboxSize()
        {
            if (!enabled) return;
            CharacterBody cb = RoR2Content.Survivors.Bandit2.bodyPrefab.GetComponent<CharacterBody>();
            ChildLocator childLocator = cb.modelLocator.modelTransform.GetComponent<ChildLocator>();
        }
    }
}
