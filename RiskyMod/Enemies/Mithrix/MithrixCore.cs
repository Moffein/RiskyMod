using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.Mithrix
{
    public class MithrixCore
    {
        public static bool enabled = true;

        public MithrixCore()
        {
            if (!enabled) return;
            new MithrixFallImmune();
            new MithrixTargetPrioritization();
            new SprintBashAntiTrimp();
        }
    }
}
