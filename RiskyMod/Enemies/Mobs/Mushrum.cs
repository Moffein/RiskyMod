using UnityEngine;
using RoR2;
using RoR2.Projectile;
using R2API;
using UnityEngine.AddressableAssets;

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
            ReduceProcCoefficient();
        }

        private void ReduceProcCoefficient()
        {
            modifiedGas = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("RiskyModMushrumProjectileDotZone", true);
            ProjectileDotZone pdz = modifiedGas.GetComponent<ProjectileDotZone>();
            pdz.overlapProcCoefficient = 0.2f;  //0.5 is vanilla

            modifiedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModMushrumProjectile", true);
            ProjectileImpactExplosion pie = modifiedProjectile.GetComponent<ProjectileImpactExplosion>();
            pie.childrenProjectilePrefab = modifiedGas;

            Content.Content.projectilePrefabs.Add(modifiedGas);
            Content.Content.projectilePrefabs.Add(modifiedProjectile);
        }
    }
}
