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
            EnemiesCore.DisableRegen(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/hermitcrabbody"));
        }
    }
}
