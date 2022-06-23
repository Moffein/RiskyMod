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
                if(c.TryGotoNext(
                     x => x.MatchCallvirt<ProjectileManager>("FireProjectile")
                    ))
                {
                    c.EmitDelegate<Func<FireProjectileInfo, FireProjectileInfo>>(projectileInfo =>
                    {
                        projectileInfo.position -= projectileInfo.rotation.eulerAngles.normalized;
                        return projectileInfo;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Loader ZapConePosition IL Hook failed");
                }
            };

            GameObject projectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/loaderzapcone");
            ProjectileProximityBeamController pbc = projectile.GetComponent<ProjectileProximityBeamController>();
            pbc.attackRange += 1f;
        }
    }
}
