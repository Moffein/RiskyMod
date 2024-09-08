using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Projectile;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Bosses
{
    public class Vagrant
    {
        public static bool enabled = true;
        public static bool disableProjectileOnKill = true;
        public Vagrant()
        {
            if (enabled)
            {
                DisableNovaAttackSpeed();
                ReduceFalloff();
            }
            
            if (disableProjectileOnKill)
            {
                RemoveTrackingBombOnKill();
            }
        }

        private void DisableNovaAttackSpeed()
        {
            IL.EntityStates.VagrantMonster.ChargeMegaNova.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdfld<EntityStates.BaseState>("attackSpeedStat")
                    ))
                {
                    c.Index++;
                    c.EmitDelegate<Func<float, float>>((attackSpeed) =>
                    {
                        return 1f;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Vagrant IL Hook failed");
                }
            };
        }

        private void RemoveTrackingBombOnKill()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/vagranttrackingbomb");
            HealthComponent hc = projectile.GetComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
        }

        private void ReduceFalloff()
        {
            {
                GameObject trackingBomb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantTrackingBomb.prefab").WaitForCompletion().InstantiateClone("RiskyModVagrantTrackingBomb", true);
                ProjectileExplosion pe = trackingBomb.GetComponent<ProjectileExplosion>();
                pe.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                DamageAPI.ModdedDamageTypeHolderComponent mdc = trackingBomb.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                if (!mdc) mdc = trackingBomb.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                mdc.Add(SharedDamageTypes.SweetSpotModifier);
                Content.Content.projectilePrefabs.Add(trackingBomb);
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Vagrant/EntityStates.VagrantMonster.FireTrackingBomb.asset", "projectilePrefab", trackingBomb);
            }

            {
                GameObject cannon = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantCannon.prefab").WaitForCompletion().InstantiateClone("RiskyModVagrantCannon", true);
                ProjectileExplosion pe = cannon.GetComponent<ProjectileExplosion>();
                pe.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                DamageAPI.ModdedDamageTypeHolderComponent mdc = cannon.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                if (!mdc) mdc = cannon.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                mdc.Add(SharedDamageTypes.SweetSpotModifier);
                Content.Content.projectilePrefabs.Add(cannon);
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Vagrant/EntityStates.VagrantMonster.Weapon.JellyBarrage.asset", "projectilePrefab", cannon);
            }
        }
    }
}
