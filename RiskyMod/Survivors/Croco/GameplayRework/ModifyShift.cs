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
            ModifyDamageType();
            ModifyAcid();
            ChainableLeapCooldown();
        }

        private void ModifyDamageType()
        {
            IL.EntityStates.Croco.BaseLeap.DetonateAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    ))
                {
                    c.EmitDelegate<Func<BlastAttack, BlastAttack>>(orig =>
                    {
                        orig.damageType = DamageType.Stun1s;
                        orig.AddModdedDamageType(SharedDamageTypes.AntiFlyingForce);
                        orig.AddModdedDamageType(SharedDamageTypes.CrocoBlight6s);
                        return orig;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Croco ModifyShift BaseLeap IL Hook failed");
                }
            };
        }

        private void ModifyAcid()
        {
            //Note: Acid projectile's damage coefficient is 1f
            AcidPuddleProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocoleapacid").InstantiateClone("RiskyMod_CrocoLeapAcid", true);

            //Ticks twice per second.
            ProjectileDotZone pdz = AcidPuddleProjectile.GetComponent<ProjectileDotZone>();
            //pdz.damageCoefficient = 0.45f;    //default is 0.25, ticks twice per second
            pdz.overlapProcCoefficient = 0.5f;  //default is 0.1

            Content.Content.projectilePrefabs.Add(AcidPuddleProjectile);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.BaseLeap", "projectilePrefab", AcidPuddleProjectile);
        }

        private void ChainableLeapCooldown()
        {
            float cdr = 0.7f;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.ChainableLeap", "refundPerHit", "0");
            IL.EntityStates.Croco.ChainableLeap.DoImpactAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchStloc(0)
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);    //self
                    c.EmitDelegate<Func<BlastAttack.Result, EntityStates.Croco.ChainableLeap, BlastAttack.Result>>((result, self) =>
                    {
                        self.skillLocator.primary.RunRecharge((float)result.hitCount * cdr);
                        self.skillLocator.secondary.RunRecharge((float)result.hitCount * cdr);
                        self.skillLocator.utility.RunRecharge((float)result.hitCount * cdr);
                        self.skillLocator.special.RunRecharge((float)result.hitCount * cdr);
                        return result;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Croco ModifyShift ChainableLeap IL Hook failed");
                }
            };
        }
    }
}
