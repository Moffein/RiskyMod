﻿using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.Holdouts
{
    public class SmallHoldoutRadius
    {
        public static bool enabled = true;
        public SmallHoldoutRadius()
        {
            if (!enabled) return;

            On.RoR2.HoldoutZoneController.Awake += (orig, self) =>
            {
                SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                if (sd)
                {
                    if ( sd.baseSceneName.Equals("arena"))
                    {
                        self.baseRadius *= 4f / 3f;
                    }
                    else if (sd.baseSceneName.Equals("moon2") && self.name.Contains("MoonBattery"))
                    {
                        self.baseRadius *= 4f / 3f;
                    }
                    else if (sd.baseSceneName.Equals("voidstage"))
                    {
                        self.baseRadius *= 4f / 3f;
                    }
                }
                orig(self);
            };
        }
    }
}