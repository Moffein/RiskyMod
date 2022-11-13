using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.Legendary
{
    public class Aegis
    {
        public static bool enabled = true;

        public Aegis()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Stacks no longer give extra barrier. Initial stack barrier increased.
            IL.RoR2.HealthComponent.Heal += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "barrierOnOverHeal")
                    ))
                {
                    if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "barrierOnOverHeal")
                    ))
                    {
                        c.EmitDelegate<Func<int, int>>(itemCount => 1);
                        error = false;

                        /*if (c.TryGotoNext(
                         x => x.MatchLdcR4(0.5f)
                        ))
                        {
                            c.Next.Operand = 1f;
                        }*/
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Aegis IL Hook failed");
                }
            };

            //Apply barrier decay rate modifiers.
            IL.RoR2.HealthComponent.ServerFixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCallvirt<CharacterBody>("get_barrierDecayRate")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, HealthComponent, float>>((decayRate, self) =>
                    {
                        int aegisCount = self.itemCounts.barrierOnOverHeal;
                        return decayRate / (1f + aegisCount);
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Aegis BarrierDecay IL Hook failed");
                }
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BarrierOnOverHeal);
        }
    }
}
