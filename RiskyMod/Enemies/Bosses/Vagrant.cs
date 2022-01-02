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
        public Vagrant()
        {
            if (!enabled) return;
            //ModifyPrimaryProjectile();
            DisableNovaAttackSpeed();
            RemoveTrackingBombOnKill();
        }

        //Disabled because this doesn't play well. Projectiles too slow to be a threat when the radius is reduced.
        private void ModifyBarrage()
        {
            /*GameObject projectile = Resources.Load<GameObject>("prefabs/projectiles/vagrantcannon");
            ProjectileImpactExplosion pie = projectile.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = 6f;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            ProjectileAPI.Add(projectile);
            SneedUtils.SneedUtils.SetEntityStateField("entitystates.vagrantmonster.weapon.jellybarrage", "projectilePrefab", projectile);*/

            //SneedUtils.SneedUtils.SetEntityStateField("entitystates.vagrantmonster.weapon.jellybarrage", "damageCoefficient", "4"); //orig is 4

            //SneedUtils.SneedUtils.DumpEntityStateConfig("entitystates.vagrantmonster.weapon.jellybarrage");
            /*
                [Info   : Unity Log] baseDuration - 2
                [Info   : Unity Log] muzzleString - MissileLaunchCenter
                [Info   : Unity Log] missileSpawnFrequency - 3
                [Info   : Unity Log] missileSpawnDelay - 0
                [Info   : Unity Log] damageCoefficient - 4
                [Info   : Unity Log] maxSpread - 1
                [Info   : Unity Log] projectilePrefab - VagrantCannon (UnityEngine.GameObject)
                [Info   : Unity Log] muzzleflashPrefab - MuzzleflashHermitCrab (UnityEngine.GameObject)
             */
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
