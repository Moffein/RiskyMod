using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Captain
{
    public class CaptainCore
    {
        public static bool enabled = true;
        public CaptainCore()
        {
            if (!enabled) return;
            new Shock();
            new Microbots();
            new CaptainOrbitalHiddenRealms();
        }
    }
}
