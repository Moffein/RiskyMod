using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.DLC1.Voidling
{
    public class VoidlingFogDamage
    {
        public static bool enabled = true;
        public VoidlingFogDamage()
        {
            if (!enabled) return;
        }
    }
}
