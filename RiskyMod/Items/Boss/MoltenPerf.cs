using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace RiskyMod.Items.Boss
{
    public class MoltenPerf
    {
        public static bool enabled = true;
        public static float initialDamageCoefficient = 3f;
        public static float stackDamageCoefficient = 2.1f;
        public static GameObject meatballPrefab;

        public MoltenPerf()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            float initialDamage = initialDamageCoefficient - stackDamageCoefficient;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireballsOnHit")
                    )
                &&
                c.TryGotoNext(
                     x => x.MatchLdfld<DamageInfo>("damage")
                    ))
                {
                    c.Index -= 6;
                    c.Next.Operand = MoltenPerf.stackDamageCoefficient;
                    c.Index += 4;
                    c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                    {
                        return damageCoefficient + initialDamage;
                    });


                    if (RiskyMod.disableProcChains)
                    {
                        if (c.TryGotoNext(
                            x => x.MatchLdstr("Prefabs/Projectiles/FireMeatBall")
                            ))
                        {
                            c.Index += 2;
                            c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                            {
                                return MoltenPerf.meatballPrefab;
                            });
                        }
                    }
                    error = false;
                }

               if (error)
               {
                    UnityEngine.Debug.LogError("RiskyMod: MoltenPerf IL Hook failed");
               }
            };

            if (RiskyMod.disableProcChains)
            {
                meatballPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireMeatBall").InstantiateClone("RiskyMod_Perforator", true);
                ProjectileImpactExplosion pie = meatballPrefab.GetComponent<ProjectileImpactExplosion>();
                pie.blastProcCoefficient = 0f;
                DamageAPI.ModdedDamageTypeHolderComponent mdc = meatballPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                mdc.Add(SharedDamageTypes.AlwaysIgnite);
                Content.Content.projectilePrefabs.Add(meatballPrefab);
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.FireballsOnHit);
        }
    }
}
