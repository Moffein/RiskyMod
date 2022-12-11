using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class BarrierDecay
    {
        public static bool enabled = true;
        public static float minDecay = 0.3f;
        public static float maxDecay = 1.5f;

        public BarrierDecay()
        {
            if (!enabled) return;

            IL.RoR2.HealthComponent.ServerFixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(MoveType.After,
                     x => x.MatchCallvirt<CharacterBody>("get_barrierDecayRate")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, HealthComponent, float>>((decayRate, self) =>
                    {
                        float barrierPercent = self.barrier / self.fullCombinedHealth;
                        return decayRate * Mathf.Lerp(minDecay, maxDecay, barrierPercent);
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: BarrierDecay IL Hook failed");
                }
            };
        }
    }
}
