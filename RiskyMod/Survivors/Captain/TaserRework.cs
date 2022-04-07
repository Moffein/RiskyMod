using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Captain
{
    public class TaserRework
    {
        public static bool enabled = true;
        public TaserRework()
        {
            if (!enabled) return;

            GameObject taserPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/CaptainTazer").InstantiateClone("RiskyModCaptainTazer", true);
            DamageAPI.ModdedDamageTypeHolderComponent mdc = taserPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(SharedDamageTypes.CaptainTaserSource);

            ProjectileDamage pd = taserPrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Shock5s;

            /*ProjectileImpactExplosion pie = taserPrefab.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = 3f;*/

            UnityEngine.Object.Destroy(taserPrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileSingleTargetImpact psi = taserPrefab.AddComponent<ProjectileSingleTargetImpact>();
            psi.destroyOnWorld = false;
            psi.destroyWhenNotAlive = true;
            psi.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazerNova.prefab").WaitForCompletion();

            Content.Content.projectilePrefabs.Add(taserPrefab);

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Captain.Weapon.FireTazer", "projectilePrefab", taserPrefab);
        }
    }
}
