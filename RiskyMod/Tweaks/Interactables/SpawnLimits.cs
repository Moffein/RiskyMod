using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Tweaks
{
    public class SpawnLimits
    {
        public static bool enabled = true;

        public static int maxVoidSeeds = 1;

        public SpawnLimits()
        {
            if (!enabled) return;

            if (maxVoidSeeds != 3) Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion().maxSpawnsPerStage = maxVoidSeeds;
        }
    }
}
