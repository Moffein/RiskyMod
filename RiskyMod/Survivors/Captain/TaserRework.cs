using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Captain
{
    public class TaserRework
    {
        public TaserRework()
        {
            GameObject taserPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazer.prefab").WaitForCompletion().InstantiateClone("RiskyModCaptainTazer", true);
            DamageAPI.ModdedDamageTypeHolderComponent mdc = taserPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(SharedDamageTypes.CaptainTaserSource);
            mdc.Add(SharedDamageTypes.Slow50For5s);

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

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Captain/EntityStates.Captain.Weapon.FireTazer.asset", "projectilePrefab", taserPrefab);
        }
    }
}
