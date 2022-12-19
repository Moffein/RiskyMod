using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.DLC1.Voidling
{
    public class VoidlingCore
    {
        public static bool enabled = true;

        public VoidlingCore()
        {
            if (!enabled) return;

            new VoidlingTargetPrioritization();
            new VoidlingFogDamage();
            new VoidlingStats();
        }
    }
}
