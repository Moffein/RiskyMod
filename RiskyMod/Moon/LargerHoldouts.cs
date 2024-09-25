using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Moon
{
    public class LargerHoldouts
    {
        public static bool enabled = true;
        public LargerHoldouts()
        {
            if (!enabled) return;

            /*string[] prefabList = new string[]
            {
                "RoR2/Base/moon2/MoonBatteryTemplate.prefab",
                "RoR2/Base/moon2/MoonBatteryBlood.prefab",
                "RoR2/Base/moon2/MoonBatteryDesign.prefab",
                "RoR2/Base/moon2/MoonBatteryMass.prefab",
                "RoR2/Base/moon2/MoonBatterySoul.prefab"
            };

            foreach (string str in prefabList)
            {
                ModifyHoldout(Addressables.LoadAssetAsync<GameObject>(str).WaitForCompletion(), 30f);
            }*/

            //I give up
            On.RoR2.HoldoutZoneController.Start += ModifyMoonHoldout;

            GameObject designPulse = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon2/MoonBatteryDesignPulse.prefab").WaitForCompletion();
            if (designPulse)
            {
                PulseController p = designPulse.GetComponent<PulseController>();
                if (p) p.finalRadius = 30f;
            }
        }

        private void ModifyMoonHoldout(On.RoR2.HoldoutZoneController.orig_Start orig, HoldoutZoneController self)
        {
            if (self.inBoundsObjectiveToken.Contains("OBJECTIVE_MOON_BATTERY"))
            {
                self.baseRadius = 30f;
            }
            orig(self);
        }

        /*private void ModifyHoldout(GameObject gameObject, float radiusOverride)
        {
            if (!gameObject) return;

            HoldoutZoneController hzc = gameObject.GetComponent<HoldoutZoneController>();
            if (!hzc) return;

            hzc.baseRadius = radiusOverride;
        }*/
    }
}
