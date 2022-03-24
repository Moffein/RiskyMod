using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace RiskyMod.Enemies.Mobs
{
    public class Mushrum
    {
        public static bool enabled = true;
        public Mushrum()
        {
            if (!enabled) return;
            EnemiesCore.DisableRegen(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/minimushroombody"));
            ReduceProcCoefficient();
        }

        private void ReduceProcCoefficient()
        {
            GameObject gasProjectile = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectileDotZone");
            ProjectileDotZone pdz = gasProjectile.GetComponent<ProjectileDotZone>();
            pdz.overlapProcCoefficient = 0.2f;  //0.5 is vanilla
        }
    }
}
