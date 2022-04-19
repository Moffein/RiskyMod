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

        public static int maxMountainShrines = 3;
        public static int maxCombatShrines = 3;
        public static int maxVoidSeeds = 1;

        public SpawnLimits()
        {
            if (!enabled) return;

            if (maxMountainShrines >= 0)
            {
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion().maxSpawnsPerStage = maxMountainShrines;
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBossSandy.asset").WaitForCompletion().maxSpawnsPerStage = maxMountainShrines;
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBossSnowy.asset").WaitForCompletion().maxSpawnsPerStage = maxMountainShrines;
            }

            if (maxCombatShrines >= 0)
            {
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombat.asset").WaitForCompletion().maxSpawnsPerStage = maxCombatShrines;
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombatSandy.asset").WaitForCompletion().maxSpawnsPerStage = maxCombatShrines;
                Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombatSnowy.asset").WaitForCompletion().maxSpawnsPerStage = maxCombatShrines;
            }

            //Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion().maxSpawnsPerStage = maxVoidSeeds;
        }
    }
}
