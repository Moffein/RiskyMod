using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Loader
{
    public class ZapFistLightningDamageScaling
    {
        public ZapFistLightningDamageScaling()
        {
            IL.EntityStates.Loader.SwingZapFist.OnMeleeHitAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<ProjectileManager>("FireProjectile")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<FireProjectileInfo, EntityStates.Loader.SwingZapFist, FireProjectileInfo>>((projectileInfo, self) =>
                {
                    projectileInfo.damage = self.bonusDamage * 2f / 27f; //2/3 * 1/9
                    return projectileInfo;
                });
            };
        }
    }
}
