using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.VoidLocus
{
    public class VoidLocusCore
    {
        public static bool enabled = true;
        public VoidLocusCore()
        {
            if (!enabled) return;
            new RemoveFog();
            new FogDamage();
            new ModifyHoldout();
        }
    }
}
