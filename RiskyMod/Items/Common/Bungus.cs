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
                c.GotoNext(
                     x => x.MatchLdcR4(1f)
                    );
                c.Next.Operand = 0.5f;
            };
        }
    }
}
