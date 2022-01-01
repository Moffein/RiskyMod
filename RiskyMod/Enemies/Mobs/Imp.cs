using UnityEngine;
using RoR2;

namespace RiskyMod.Enemies.Mobs
{
    public class Imp
    {
        public static bool enabled = true;
        public Imp()
        {
            if (!enabled) return;
            DisableRegen();
        }

        private void DisableRegen()
        {
            GameObject enemyObject = Resources.Load<GameObject>("prefabs/characterbodies/impbody");
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();
            cb.baseRegen = 0f;
            cb.levelRegen = 0f;
        }
    }
}
