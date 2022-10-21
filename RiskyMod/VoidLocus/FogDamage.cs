using RoR2;
using UnityEngine;

namespace RiskyMod.VoidLocus
{
    public class FogDamage
    {
        public static bool enabled = true;

        public FogDamage()
        {
            if (!enabled || RemoveFog.enabled) return;
            On.RoR2.VoidStageMissionController.RequestFog += (orig, self, zone) =>
            {
                if (self.fogDamageController)
                {
                    //Debug.Log("HP% per second: " + self.fogDamageController.healthFractionPerSecond);   //0.05
                    //Debug.Log("HP% Ramping per second: " + self.fogDamageController.healthFractionRampCoefficientPerSecond);    //0.1

                    self.fogDamageController.healthFractionPerSecond = 0.025f;
                    self.fogDamageController.healthFractionRampCoefficientPerSecond = 0f;
                }
                return orig(self, zone);
            };
        }
    }
}
