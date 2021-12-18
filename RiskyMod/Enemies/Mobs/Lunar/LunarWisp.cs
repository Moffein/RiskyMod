using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;


namespace RiskyMod.Enemies.Mobs.Lunar
{
    public class LunarWisp
    {
        public static bool enabled = true;
        public LunarWisp()
        {
            if (!enabled) return;
            RemoveTrackingBombOnKill();
        }

        private void RemoveTrackingBombOnKill()
        {
            GameObject projectile = Resources.Load<GameObject>("prefabs/projectiles/lunarwisptrackingbomb");
            HealthComponent hc = projectile.GetComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
        }
    }
}
