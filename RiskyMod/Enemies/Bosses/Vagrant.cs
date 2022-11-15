using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Projectile;
using R2API;
using UnityEngine;

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
    }
}
