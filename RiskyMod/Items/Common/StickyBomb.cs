using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RoR2.Projectile;
using UnityEngine;
using System;

namespace RiskyMod.Items.Common
{
    public class StickyBomb
    {
        public static bool enabled = true;
        public static GameObject stickybombPrefab;

        public StickyBomb()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "StickyBomb")
                    ))
                {
                    //New Damage
                    if(c.TryGotoNext(
                    x => x.MatchLdcR4(1.8f)
                    ))
                    {
                        c.Next.Operand = 2.4f;

                        //Replace projectile
                        if(c.TryGotoNext(
                            x => x.MatchLdstr("Prefabs/Projectiles/StickyBomb")
                            ))
                        {
                            c.Index += 2;
                            c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                            {
                                return StickyBomb.stickybombPrefab;
                            });

                            error = false;
                        }
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: StickyBomb IL Hook failed");
                }
            };

            //Modify detonation delay
            stickybombPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/stickybomb"); //No longer clones the GameObject since changes are minimal.
            ProjectileImpactExplosion pie = stickybombPrefab.GetComponent<ProjectileImpactExplosion>();
            pie.lifetime = 1.2f;
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.StickyBomb);
        }
    }
}
