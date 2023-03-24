using UnityEngine;
using RoR2;
using RoR2.Projectile;
using R2API;

namespace RiskyMod.Enemies.Mobs
{
    public class Mushrum
    {
        public static bool enabled = true;

        public static GameObject modifiedProjectile;
        public static GameObject modifiedGas;

        public Mushrum()
        {
            if (!enabled) return;
            EnemiesCore.DisableRegen(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/minimushroombody"));
            ReduceProcCoefficient();
        }

        private void ReduceProcCoefficient()
        {
            modifiedGas = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectileDotZone").InstantiateClone("RiskyModMushrumProjectileDotZone", true);
            ProjectileDotZone pdz = modifiedGas.GetComponent<ProjectileDotZone>();
            pdz.overlapProcCoefficient = 0.2f;  //0.5 is vanilla
            Allies.DotZoneResist.AddDotZoneDamageType(modifiedGas);

            modifiedProjectile = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectile").InstantiateClone("RiskyModMushrumProjectile", true);
            ProjectileImpactExplosion pie = modifiedProjectile.GetComponent<ProjectileImpactExplosion>();
            pie.childrenProjectilePrefab = modifiedGas;

            Content.Content.projectilePrefabs.Add(modifiedGas);
            Content.Content.projectilePrefabs.Add(modifiedProjectile);
        }
    }
}
