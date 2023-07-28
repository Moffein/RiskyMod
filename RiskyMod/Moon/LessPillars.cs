using MonoMod.Cil;
using UnityEngine.Networking;
using RoR2;
using System;
using UnityEngine.Events;
using EntityStates.Missions.Moon;
using EntityStates.MoonElevator;

namespace RiskyMod.Moon
{
    public class LessPillars
    {
        public static bool enabled = true;
        public static int requiredBatteries = 2;
        public LessPillars()
        {
            if (!enabled) return;
            On.RoR2.MoonBatteryMissionController.Awake += (orig, self) =>
            {
                orig(self);
                self._numRequiredBatteries = requiredBatteries;
			};
        }
    }
}
