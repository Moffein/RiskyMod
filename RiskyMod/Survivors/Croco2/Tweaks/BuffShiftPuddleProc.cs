using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco2.Tweaks
{
    public class BuffShiftPuddleProc
    {
        public static bool enabled = true;

        public BuffShiftPuddleProc()
        {
            if (!enabled) return;

            GameObject acid = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoLeapAcid.prefab").WaitForCompletion();
            acid.GetComponent<ProjectileDotZone>().overlapProcCoefficient = 0.7f;
        }
    }
}
