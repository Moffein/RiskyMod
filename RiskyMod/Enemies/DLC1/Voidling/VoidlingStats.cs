using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;

namespace RiskyMod.Enemies.DLC1.Voidling
{
    public class VoidlingStats
    {
        public static bool modifyHP = true;

        public VoidlingStats()
        {
            if (modifyHP)
            {
                SetHP(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyBase.prefab").WaitForCompletion().GetComponent<CharacterBody>());
                SetHP(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyPhase1.prefab").WaitForCompletion().GetComponent<CharacterBody>());
                SetHP(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyPhase2.prefab").WaitForCompletion().GetComponent<CharacterBody>());
                SetHP(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyPhase3.prefab").WaitForCompletion().GetComponent<CharacterBody>());
            }
        }

        private void SetHP(CharacterBody cb)
        {
            cb.baseMaxHealth = 1400f;   //2000 vanilla
            cb.levelMaxHealth = 420f;
        }
    }
}
