using RoR2.Projectile;
using UnityEngine;
using RoR2;
using MonoMod.Cil;
using System;

namespace RiskyMod.Survivors.Loader
{
    public class ZapConePosition
    {
        public ZapConePosition()
        {
            IL.EntityStates.Loader.SwingZapFist.OnMeleeHitAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<ProjectileManager>("FireProjectile")
                    );
                c.EmitDelegate<Func<FireProjectileInfo, FireProjectileInfo>>(projectileInfo =>
                {
                    projectileInfo.position -= projectileInfo.rotation.eulerAngles.normalized;
                    return projectileInfo;
                });
            };

            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/loaderzapcone");
            ProjectileProximityBeamController pbc = projectile.GetComponent<ProjectileProximityBeamController>();
            pbc.attackRange += 1f;
        }
    }
}
