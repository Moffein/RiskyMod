using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC1.Void
{
    public class Polylute
    {
        public static bool enabled = true;
        public Polylute()
        {
            if (!enabled || !RiskyMod.disableProcChains) return;
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(0.2f)
                    );
                c.Next.Operand = 0f;
            };
        }
    }
}
