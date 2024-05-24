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
                    //This extra check is for Moon2's Escape ship, which has the generic name "HoldoutZone"
                    bool isMoon = false;
                    SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                    if (sd.baseSceneName.Equals("moon2") || sd.baseSceneName.Equals("moon"))
                    {
                        isMoon = true;
                    }

                    if (isMoon
                    || holdoutZoneController.name.Contains("NullSafeZone")  //Void Fields
                    || holdoutZoneController.name.Contains("Battery")   //Moon and Void Locus
                    || holdoutZoneController.name.Contains("Pillar"))   //Used for PillarRevive
                    {
                        return HoldoutZoneController.CountLivingPlayers(teamIndex);
                    }
                }
                return players;
            };
        }
    }
}
