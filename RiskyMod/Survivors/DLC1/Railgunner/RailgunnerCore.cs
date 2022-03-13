using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.DLC1.Railgunner
{
    class RailgunnerCore
    {
        public static bool enabled = true;
        public static bool fixBungus = true;

        public RailgunnerCore()
        {
            if (!enabled) return;
            new FixBungus();
        }
    }
}
