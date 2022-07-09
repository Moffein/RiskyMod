using RoR2;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;

namespace RiskyMod.Tweaks.RunScaling
{
    public class LoopTeleMountainShrine
    {
        public static bool enabled = false;
        public LoopTeleMountainShrine()
        {
            if (!enabled) return;

            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCallvirt<TeleporterInteraction>("get_shrineBonusStacks")
                    ))
                {
                    c.EmitDelegate<Func<int, int>>((orig) =>
                    {
                        return orig + Mathf.FloorToInt(Run.instance.stageClearCount / 5f);
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: LoopTeleMountainShrine IL Hook failed");
                }
            };
        }
    }
}
