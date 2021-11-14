using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks
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
                    //if (holdoutZoneController.name.Contains("NullSafeZone") || holdoutZoneController.name.Contains("MoonBattery"))
                    SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                    if (sd && (sd.baseSceneName.Equals("arena") || sd.baseSceneName.Equals("moon2")))
                    {
                        return HoldoutZoneController.CountLivingPlayers(teamIndex);
                    }
                }
                return players;
            };
        }
    }
}
