using RiskyMod.Survivors.Croco.Tweaks;
using RiskyMod.Survivors.Croco.Contagion;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco
{
    public class CrocoCore
    {
        public static bool enabled = true;
        public static GameObject bodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();

        public CrocoCore()
        {
            if (!enabled) return;

            new UtilityKnockdown();
            new BuffFrenziedLeap();
            new BuffShiftPuddleProc();
            new BiggerLeapHitbox();
            new ShiftAirControl();
            new ContagionPassive();
            new BlightReturns();
        }
    }
}
