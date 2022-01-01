using UnityEngine;
using RoR2;

namespace RiskyMod.Enemies.Mobs
{
    public class HermitCrab
    {
        public static bool enabled = true;
        public HermitCrab()
        {
            if (!enabled) return;
            DisableRegen();
        }

        private void DisableRegen()
        {
            GameObject enemyObject = Resources.Load<GameObject>("prefabs/characterbodies/hermitcrabbody");
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();
            cb.baseRegen = 0f;
            cb.levelRegen = 0f;
        }
    }
}
