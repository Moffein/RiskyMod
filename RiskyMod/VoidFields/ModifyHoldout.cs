using RoR2;
using UnityEngine;

namespace RiskyMod.VoidFields
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
                    if (sd.baseSceneName.Equals("arena"))
                    {
                        self.baseRadius *= 2f;  //15f in vanilla
                        self.baseChargeDuration *= 0.6666666667f;  //60f in vanilla
                    }
                }
                orig(self);
            };
        }
    }
}
