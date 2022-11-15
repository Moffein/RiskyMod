using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;


namespace RiskyMod.Enemies.Mobs.Lunar
{
    public class LunarWisp
    {
        public static bool enabled = true;
        public static bool disableProjectileOnKill = true;
        public static bool enableFalloff = true;

        public LunarWisp()
        {
            if (enabled)
            {
                EnableStatusConditions();
                EnableBulletFalloff();
            }
            ModifyProjectile();
        }

        private void EnableStatusConditions()
        {
            GameObject enemyObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/lunarwispbody");
            SetStateOnHurt ssoh = enemyObject.GetComponent<SetStateOnHurt>();
            if (!ssoh)
            {
                ssoh = enemyObject.AddComponent<SetStateOnHurt>();
            }
            ssoh.hitThreshold = 0.5f;
            ssoh.canBeHitStunned = true;
            ssoh.canBeStunned = true;
            ssoh.canBeFrozen = true;

            EntityStateMachine body = null;
            EntityStateMachine weapon = null;
            EntityStateMachine[] stateMachines = enemyObject.GetComponents<EntityStateMachine>();
            foreach(EntityStateMachine esm in stateMachines)
            {
                switch(esm.customName)
                {
                    case "Body":
                        body = esm;
                        break;
                    case "Weapon":
                        weapon = esm;
                        break;
                    default:
                        break;
                }
            }

            ssoh.targetStateMachine = body;
            ssoh.idleStateMachine = new EntityStateMachine[] { weapon };
            ssoh.hurtState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.HurtStateFlyer));
        }

        private void ModifyProjectile()
        {
            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/lunarwisptrackingbomb");

            if (disableProjectileOnKill)
            {
                HealthComponent hc = projectile.GetComponent<HealthComponent>();
                hc.globalDeathEventChanceCoefficient = 0f;
            }

            if (enabled)
            {
                ProjectileImpactExplosion pie = projectile.GetComponent<ProjectileImpactExplosion>();
                pie.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            }
        }

        private void EnableBulletFalloff()
        {
            IL.EntityStates.LunarWisp.FireLunarGuns.OnFireAuthority += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                    ))
                {
                    c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                    {
                        bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                        return bulletAttack;
                    });

                    if (c.TryGotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                    ))
                    {
                        c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                        {
                            bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                            return bulletAttack;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: LunarWisp IL Hook failed");
                }
            };
        }
    }
}
