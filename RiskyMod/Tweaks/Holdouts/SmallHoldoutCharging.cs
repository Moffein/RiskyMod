using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.Holdouts
{
    public class SmallHoldoutCharging
    {
        public static bool enabled = true;
        public SmallHoldoutCharging()
        {
            if (!enabled) return;
            On.RoR2.HoldoutZoneController.CountPlayersInRadius += (orig, holdoutZoneController, origin, chargingRadiusSqr, teamIndex) =>
            {
                int players = orig(holdoutZoneController, origin, chargingRadiusSqr, teamIndex);
                if (players > 0)
                {
                    bool isMoon = false;
                    SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                    if (sd.baseSceneName.Equals("moon2") || sd.baseSceneName.Equals("moon"))
                    {
                        isMoon = true;
                    }

                    if (isMoon || holdoutZoneController.name.Contains("NullSafeZone") || holdoutZoneController.name.Contains("Battery"))
                    {
                        return HoldoutZoneController.CountLivingPlayers(teamIndex);
                    }
                }
                return players;
            };
        }
    }
}
