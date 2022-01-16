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
            EnemiesCore.DisableRegen(Resources.Load<GameObject>("prefabs/characterbodies/impbody"));
        }

        private void ModifyAI()
        {

        }
    }
}
