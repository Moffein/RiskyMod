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
            EnemiesCore.DisableRegen(Resources.Load<GameObject>("prefabs/characterbodies/hermitcrabbody"));
        }
    }
}
