using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyMod.Enemies.Bosses
{
    public class Vagrant
    {
        public static bool enabled = true;
        public Vagrant()
        {
            if (!enabled) return;
            DisableNovaAttackSpeed();
            RemoveTrackingBombOnKill();
        }

        private void DisableNovaAttackSpeed()
        {
            IL.EntityStates.VagrantMonster.ChargeMegaNova.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdfld<EntityStates.BaseState>("attackSpeedStat")
                    );
                c.Index++;
                c.EmitDelegate<Func<float, float>>((attackSpeed) =>
                {
                    return 1f;
                });
            };
        }

        private void RemoveTrackingBombOnKill()
        {
            GameObject projectile = Resources.Load<GameObject>("prefabs/projectiles/vagranttrackingbomb");
            HealthComponent hc = projectile.GetComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
        }
    }
}
