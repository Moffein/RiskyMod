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

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                BodyIndex brotherIndex = BodyCatalog.FindBodyIndex("BrotherBody");
                if(brotherIndex != BodyIndex.None) PrioritizePlayers.prioritizePlayersList.Add(brotherIndex);
            };
        }
    }
}
