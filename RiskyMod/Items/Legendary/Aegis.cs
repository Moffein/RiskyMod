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
