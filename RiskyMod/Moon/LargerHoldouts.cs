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

            string[] prefabList = new string[]
            {
                "RoR2/Base/moon2/MoonBatteryBlood.prefab",
                "RoR2/Base/moon2/MoonBatteryDesign.prefab",
                "RoR2/Base/moon2/MoonBatteryMass.prefab",
                "RoR2/Base/moon2/MoonBatterySoul.prefab"
            };

            foreach (string str in prefabList)
            {
                ModifyHoldout(Addressables.LoadAssetAsync<GameObject>(str).WaitForCompletion(), 30f);
            }

            GameObject designPulse = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon2/MoonBatteryDesignPulse.prefab").WaitForCompletion();
            if (designPulse)
            {
                PulseController p = designPulse.GetComponent<PulseController>();
                if (p) p.finalRadius = 30f;
            }
        }

        private void ModifyHoldout(GameObject gameObject, float radiusOverride)
        {
            if (!gameObject) return;

            HoldoutZoneController hzc = gameObject.GetComponent<HoldoutZoneController>();
            if (!hzc) return;

            hzc.baseRadius *= radiusOverride;
        }
    }
}
