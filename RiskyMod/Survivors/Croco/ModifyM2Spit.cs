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
            //md.Add(SharedDamageTypes.InterruptOnHit);
            //Interrupt on a character with 2x stuns on demand is a bit much. Would need a much longer cooldown to be justifiable, which would kill Acrid's ability to kill Wisps.

            md.Add(SharedDamageTypes.Blight7s);

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
