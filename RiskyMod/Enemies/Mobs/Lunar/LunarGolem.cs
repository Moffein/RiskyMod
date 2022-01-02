using RoR2;
using UnityEngine;

namespace RiskyMod.Enemies.Mobs.Lunar
{
    public class LunarGolem
    {
        public static bool enabled = true;
        public LunarGolem()
        {
            if (!enabled) return;
            DisableRegen();
        }

        private void DisableRegen()
        {
            GameObject enemyObject = Resources.Load<GameObject>("prefabs/characterbodies/lunargolembody");
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();
            cb.baseRegen = 0f;
            cb.levelRegen = 0f;
        }
    }
}
