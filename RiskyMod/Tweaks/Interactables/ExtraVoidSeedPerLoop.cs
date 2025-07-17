using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Tweaks.Interactables
{
    public class ExtraVoidSeedPerLoop
    {
        public static int maxExtraSeeds = 2;

        private static InteractableSpawnCard voidSeedCard = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion();

        public ExtraVoidSeedPerLoop()
        {
            if (maxExtraSeeds == 0) return;

            On.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        private void SceneDirector_PopulateScene(On.RoR2.SceneDirector.orig_PopulateScene orig, SceneDirector self)
        {
            voidSeedCard.maxSpawnsPerStage = SpawnLimits.enabled ? SpawnLimits.maxVoidSeeds : 3;
            int extraSeeds = 0;
            if (Run.instance) extraSeeds = Run.instance.stageClearCount % 5;
            if (maxExtraSeeds > 0) extraSeeds = Mathf.Min(extraSeeds, maxExtraSeeds);
            voidSeedCard.maxSpawnsPerStage += extraSeeds;

            orig(self);
        }
    }
}
