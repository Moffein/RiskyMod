using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.VoidLocus
{
    public class LargerHoldout
    {
        public static bool enabled = true;
        public LargerHoldout()
        {
            if (!enabled) return;

            GameObject gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DeepVoidPortalBattery/DeepVoidPortalBattery.prefab").WaitForCompletion();
            HoldoutZoneController hzc = gameObject.GetComponent<HoldoutZoneController>();
            if (!hzc) return;
            hzc.baseRadius = 30f;   //20f vanilla
        }
    }
}
