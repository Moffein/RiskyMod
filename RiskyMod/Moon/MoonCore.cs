using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Moon
{
    public class MoonCore
    {
        public static bool enabled = true;
        public MoonCore()
        {
            if (!enabled) return;
            new LessPillars();
            new PillarsDropItems();
        }
    }
}
