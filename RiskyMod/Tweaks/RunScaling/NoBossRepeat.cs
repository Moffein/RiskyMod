using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Tweaks.RunScaling
{
    public class NoBossRepeat
    {
        public static bool enabled = true;
        public NoBossRepeat()
        {
            if (!enabled) return;
        }
    }
}
