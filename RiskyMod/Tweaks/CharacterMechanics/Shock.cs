using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class Shock
    {
        public static bool enabled = true;
        public static float shockThreshold = 1f;
        public Shock()
        {
            if (!enabled) return;
            IL.EntityStates.ShockState.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld<ShockState>("healthFractionToForceExit")
                    ))
                {
                    c.Remove();
                    c.Emit<Shock>(OpCodes.Ldsfld, nameof(shockThreshold));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Shock IL Hook failed");
                }
            };
        }
    }
}
