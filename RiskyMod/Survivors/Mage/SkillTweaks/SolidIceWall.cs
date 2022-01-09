using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Mage
{
    //Based on https://github.com/Legendsmith/ThunderDownUnder/blob/master/SolidIceWall/SolidIceWallLoader.cs
    public class SolidIceWall
    {
        public static bool enabled = true;
        public SolidIceWall()
        {
            if (!enabled) return;

            GameObject projectile = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            projectile.layer = LayerIndex.world.intVal;
        }
    }
}
