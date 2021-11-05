using UnityEngine.Networking;

namespace RiskyMod.MoonRework
{
    public class LessPillars
    {
        public static bool enabled = true;
        public LessPillars()
        {
            if (!enabled) return;
            On.RoR2.MoonBatteryMissionController.Awake += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    self._numRequiredBatteries = 2;
                }
            };
        }
    }
}
