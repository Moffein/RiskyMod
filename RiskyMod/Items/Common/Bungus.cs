using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.Common
{
    public class Bungus
    {
        public static bool enabled = true;
        public Bungus()
        {
            if (!enabled) return;
            IL.RoR2.CharacterBody.GetNotMoving += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdcR4(1f)
                    ))
                {
                    c.Next.Operand = 0.5f;
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Bungus IL Hook failed");
                }
            };
        }
    }
}
