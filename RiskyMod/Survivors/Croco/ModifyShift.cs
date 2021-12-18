using System;
using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using RoR2.Projectile;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyShift
    {
        public static GameObject AcidPuddleProjectile;

        public ModifyShift()
        {
            IL.EntityStates.Croco.BaseLeap.DetonateAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>(orig =>
                {
                    orig.damageType = DamageType.Stun1s | DamageType.BlightOnHit;
                    orig.AddModdedDamageType(SharedDamageTypes.AntiFlyingForce);
                    //orig.AddModdedDamageType(SharedDamageTypes.Blight7s);
                    return orig;
                });
            };

            //Note: Acid projectile's damage coefficient is 100%
            AcidPuddleProjectile = Resources.Load<GameObject>("prefabs/projectiles/crocoleapacid").InstantiateClone("RiskyMod_CrocoLeapAcid", true);

            //AcidPuddleProjectile.transform.localScale *= 2f;  //Causes visual issues when the pool begins to disappear.

            //Ticks twice per second.
            ProjectileDotZone pdz = AcidPuddleProjectile.GetComponent<ProjectileDotZone>();
            //pdz.damageCoefficient = 0.45f;    //default is 0.25
            pdz.overlapProcCoefficient = 0.5f;  //default is 0.1

            ProjectileAPI.Add(AcidPuddleProjectile);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.BaseLeap", "projectilePrefab", AcidPuddleProjectile);
        }
    }
}
