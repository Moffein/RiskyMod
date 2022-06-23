using RoR2;
using UnityEngine;
using RoR2.Projectile;
using MonoMod.Cil;
using System;
using R2API;
using Mono.Cecil.Cil;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM2Spit
    {
        private static GameObject spitVanilla;
        private static GameObject spitModded;

        public ModifyM2Spit()
        {
            spitVanilla = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocospit");

            spitModded = spitVanilla.InstantiateClone("RiskyMod_CrocoSpit", true);
            Content.Content.projectilePrefabs.Add(spitModded);

            DamageAPI.ModdedDamageTypeHolderComponent mdc = spitModded.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(SharedDamageTypes.CrocoBlight6s);

            ProjectileDamage pd = spitModded.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            IL.EntityStates.Croco.FireSpit.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCallvirt<ProjectileManager>("FireProjectile")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<FireProjectileInfo, EntityStates.Croco.FireSpit, FireProjectileInfo>>((projectileInfo, self) =>
                    {
                        if (self.projectilePrefab == spitVanilla)
                        {
                            projectileInfo.projectilePrefab = spitModded;
                            projectileInfo.damageTypeOverride = null;
                        }
                        return projectileInfo;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Croco ModifyM2Spit IL Hook failed");
                }
            };
        }
    }
}
