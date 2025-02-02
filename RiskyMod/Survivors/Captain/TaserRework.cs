using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;
using MonoMod.Cil;
using System;

namespace RiskyMod.Survivors.Captain
{
    public class TaserRework
    {
        public TaserRework()
        {
            GameObject taserPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazer.prefab").WaitForCompletion().InstantiateClone("RiskyModCaptainTazer", true);

            UnityEngine.Object.Destroy(taserPrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileSingleTargetImpact psi = taserPrefab.AddComponent<ProjectileSingleTargetImpact>();
            psi.destroyOnWorld = false;
            psi.destroyWhenNotAlive = true;
            psi.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazerNova.prefab").WaitForCompletion();

            Content.Content.projectilePrefabs.Add(taserPrefab);

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Captain/EntityStates.Captain.Weapon.FireTazer.asset", "projectilePrefab", taserPrefab);

            //EntityState overrides this
            ProjectileDamage pd = taserPrefab.GetComponent<ProjectileDamage>();
            pd.damageType = (DamageTypeCombo)DamageType.Shock5s | DamageSource.Secondary;
            pd.damageType.AddModdedDamageType(SharedDamageTypes.Slow50For5s);
            pd.damageType.AddModdedDamageType(SharedDamageTypes.CaptainTaserSource);

            IL.EntityStates.Captain.Weapon.FireTazer.Fire += FireTazer_Fire;
        }

        private void FireTazer_Fire(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt<ProjectileManager>("FireProjectile")))
            {
                c.EmitDelegate<Func<FireProjectileInfo, FireProjectileInfo>>(info =>
                {
                    if (info.damageTypeOverride.HasValue)
                    {
                        DamageTypeCombo combo = info.damageTypeOverride.Value;
                        combo.AddModdedDamageType(SharedDamageTypes.Slow50For5s);
                        combo.AddModdedDamageType(SharedDamageTypes.CaptainTaserSource);
                        info.damageTypeOverride = combo;
                    }
                    return info;
                });
            }
        }
    }
}
