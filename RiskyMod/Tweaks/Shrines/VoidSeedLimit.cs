using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Tweaks
{
    public class VoidSeedLimit
    {
        public static int seedLimit = 1;
        public static bool enabled = true;

        public VoidSeedLimit()
        {
            if (!enabled) return;

            InteractableSpawnCard voidSeedCard = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion();
            voidSeedCard.maxSpawnsPerStage = seedLimit;
        }
    }
}
