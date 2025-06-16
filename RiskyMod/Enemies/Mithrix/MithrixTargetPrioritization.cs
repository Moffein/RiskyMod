using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace RiskyMod.Enemies.Mithrix
{
    public class MithrixTargetPrioritization
    {
        public static bool enabled = true;
        public MithrixTargetPrioritization()
        {
            if (!enabled) return;
            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherMaster.prefab").WaitForCompletion());
            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/ITBrotherMaster.prefab").WaitForCompletion());
        }
    }
}
