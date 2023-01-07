using RoR2;
using UnityEngine;

namespace RiskyMod.VoidLocus
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
                    if (sd.baseSceneName.Equals("voidstage"))
                    {
                        self.baseRadius *= 1.5f;  //20f in vanilla
                        self.baseChargeDuration *= 2f/3f;  //60f in vanilla
                    }
                }
                orig(self);
            };
        }
    }
}
