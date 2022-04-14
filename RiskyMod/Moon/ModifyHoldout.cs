using RoR2;
using UnityEngine;

namespace RiskyMod.Moon
{
    public class ModifyHoldout
    {
        public static bool enabled = true;
        public ModifyHoldout()
        {
            if (!enabled) return;
            On.RoR2.HoldoutZoneController.Awake += (orig, self) =>
            {
                SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                if (sd)
                {
                    if (sd.baseSceneName.Equals("moon2") && self.name.Contains("MoonBattery"))
                    {
                        self.baseRadius = 30f;  //all are 20 in vanilla

                        if (self.name.Contains("MoonBatteryMass"))
                        {
                            self.baseChargeDuration = 45f;
                        }
                        else if (self.name.Contains("MoonBatterySoul"))
                        {
                            self.baseChargeDuration = 20f;
                        }
                    }
                }
                orig(self);
            };
        }
    }
}
