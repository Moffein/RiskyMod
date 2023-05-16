using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.Legendary
{
    public class Behemoth
    {
        public static bool enabled = true;
        public Behemoth()
        {
            if (!enabled) return;

            IL.RoR2.GlobalEventManager.OnHitAll += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Behemoth")
                    ))
                {
                    //Change Radius
                    if (c.TryGotoNext(MoveType.After,
                    //x => x.MatchLdcR4(1.5f),
                    //x => x.MatchLdcR4(2.5f),
                    //x => x.MatchLdloc(3),
                    x => x.MatchConvR4(),
                    x => x.MatchMul(),
                    x => x.MatchAdd()
                    ))
                    {
                        c.Emit(OpCodes.Ldloc_3);    //itemCount
                        c.EmitDelegate<Func<float, int, float>>((origRadius, itemCount) =>
                        {
                            float newRadius = 0f;
                            for (int i = 0; i < itemCount; i++)
                            {
                                newRadius += Mathf.Max(1.25f, 4f * Mathf.Pow(0.625f, i));
                            }
                            return newRadius;
                        });

                        //Change Damage
                        /*if (c.TryGotoNext(MoveType.After,
                            x => x.MatchLdcR4(0.6f)
                            ))
                        {
                            c.Emit(OpCodes.Ldloc_3);    //itemCount
                            c.EmitDelegate<Func<float, int, float>>((origDamage, itemCount) =>
                            {
                                return origDamage + 0.2f * (itemCount - 1);
                            });
                        }*/
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Behemoth IL Hook failed");
                }
            };
        }
    }
}
