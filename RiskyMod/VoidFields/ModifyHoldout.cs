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

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Missions.Arena.NullWard.NullWardBaseState", "wardRadiusOn", "30");  //15 in vanilla 

            /*On.RoR2.HoldoutZoneController.Awake += (orig, self) =>
            {
                SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                if (sd)
                {
                    if (sd.baseSceneName.Equals("arena"))
                    {
                        //self.baseRadius *= 2f; //gets overridden by NullWardBaseState
                        self.baseChargeDuration *= 2f/3f;  //60f in vanilla
                    }
                }
                orig(self);
            };*/
        }
    }
}
