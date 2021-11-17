using RoR2;
using MonoMod.Cil;
using System;

namespace RiskyMod.Drones
{
    public class DronesCore
    {
        public static bool enabled = true;
        public DronesCore()
        {
            if (!enabled) return;
            new DroneScaling();
            new DroneTargeting();
            new IncreaseShotRadius();
            new VagrantResistance();
            TweakDrones();
        }

        private void TweakDrones()
        {
            new MegaDrone();
        }
    }
}
