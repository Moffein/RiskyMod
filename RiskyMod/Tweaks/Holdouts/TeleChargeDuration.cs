using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Tweaks.Holdouts
{
    public class TeleChargeDuration
    {
        public static bool enabled = false;

        public TeleChargeDuration()
        {
            if (!enabled) return;

            GameObject tele = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/Teleporter1.prefab").WaitForCompletion();
            HoldoutZoneController hzc = tele.GetComponent<HoldoutZoneController>();
            hzc.baseChargeDuration = 120f;

            GameObject lunarTele = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/LunarTeleporter Variant.prefab").WaitForCompletion();
            HoldoutZoneController lhzc = lunarTele.GetComponent<HoldoutZoneController>();
            lhzc.baseChargeDuration = 120f;
        }
    }
}
