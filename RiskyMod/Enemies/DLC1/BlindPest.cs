using RoR2;
using UnityEngine;

namespace RiskyMod.Enemies.DLC1
{
    public class BlindPest
    {
        public static bool enabled = true;
        public BlindPest()
        {
            if (!enabled) return;

            GameObject pestObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/FlyingVerminBody");
            CharacterBody cb = pestObject.GetComponent<CharacterBody>();
            cb.baseDamage = 7f;
            cb.levelDamage = cb.baseDamage * 0.2f;
        }
    }
}
