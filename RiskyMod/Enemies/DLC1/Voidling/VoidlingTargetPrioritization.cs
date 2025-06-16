using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace RiskyMod.Enemies.DLC1.Voidling
{
    public class VoidlingTargetPrioritization
    {
        public static bool enabled = true;
        public VoidlingTargetPrioritization()
        {
            if (!enabled) return;

            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterBase.prefab").WaitForCompletion());
            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase1.prefab").WaitForCompletion());
            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase2.prefab").WaitForCompletion());
            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase3.prefab").WaitForCompletion());
        }
    }
}
