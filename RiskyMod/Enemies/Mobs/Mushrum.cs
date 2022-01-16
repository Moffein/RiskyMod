using UnityEngine;
using RoR2;

namespace RiskyMod.Enemies.Mobs
{
    public class Mushrum
    {
        public static bool enabled = true;
        public Mushrum()
        {
            if (!enabled) return;
            EnemiesCore.DisableRegen(Resources.Load<GameObject>("prefabs/characterbodies/minimushroombody"));
        }
    }
}
