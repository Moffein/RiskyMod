using MonoMod.Cil;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;

namespace RiskyMod.Enemies
{
    public class AiTargetFinding
    {
        public static bool enabled = true;
        public AiTargetFinding()
        {
            if (!enabled) return;

            IL.RoR2.CharacterAI.BaseAI.FixedUpdate += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdfld(typeof(BaseAI), "fullVision")
                    ))
                {
                    c.EmitDelegate<Func<bool, bool>>(useFullVision => true);
                    if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcI4(1)
                    ))
                    {
                        c.EmitDelegate<Func<bool, bool>>(checkLOS => false);
                        error = false;
                    }
                }

                if (error)
                {
                    Debug.LogError("RiskyMod: AITargetFinding IL Hook failed.");
                }
            };
        }
    }
}
