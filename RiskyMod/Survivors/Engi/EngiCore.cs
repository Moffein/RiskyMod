using RoR2;
using UnityEngine;
using R2API;

namespace RiskyMod.Survivors.Engi
{
    public class EngiCore
    {
        public static bool enabled = true;
        public EngiCore()
        {
            if (!enabled) return;
            new PressureMines();
            new TurretChanges();
        }
    }
}
