using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            GameObject projectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/GreaterWisp/WispCannon.prefab").WaitForCompletion();
            ProjectileImpactExplosion pie = projectile.GetComponent<ProjectileImpactExplosion>();
            pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;
        }
    }
}
