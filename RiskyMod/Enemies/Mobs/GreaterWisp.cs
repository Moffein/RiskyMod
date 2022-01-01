using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace RiskyMod.Enemies.Mobs
{
    public class GreaterWisp
    {
        public static bool enabled = true;
        public GreaterWisp()
        {
            if (!enabled) return;
            ChangeFalloff();
        }

        private void ChangeFalloff()
        {
            GameObject projectile = Resources.Load<GameObject>("prefabs/projectiles/wispcannon");
            ProjectileImpactExplosion pie = projectile.GetComponent<ProjectileImpactExplosion>();
            pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;
        }
    }
}
