using RoR2;
using UnityEngine;
using RoR2.Projectile;
using MonoMod.Cil;
using System;
using R2API;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM2Spit
    {
        public ModifyM2Spit()
        {
            GameObject spitProjectile = Resources.Load<GameObject>("prefabs/projectiles/crocospit");

            Rigidbody rb = spitProjectile.GetComponent<Rigidbody>();
            rb.useGravity = true;

            ProjectileDamage pd = spitProjectile.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            DamageAPI.ModdedDamageTypeHolderComponent md = spitProjectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            md.Add(SharedDamageTypes.InterruptOnHit);

            IL.EntityStates.Croco.FireSpit.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<ProjectileManager>("FireProjectile")
                    );
                c.EmitDelegate<Func<FireProjectileInfo, FireProjectileInfo>>(orig =>
                {
                    orig.damageTypeOverride = default;
                    return orig;
                });
            };
        }
    }
}
