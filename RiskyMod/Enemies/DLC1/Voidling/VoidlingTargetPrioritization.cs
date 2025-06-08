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

            RoR2Application.onLoadFinished += OnLoad;
        }

        private void OnLoad()
        {
            BodyIndex voidlingIndex = BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyBase");
            if (voidlingIndex != BodyIndex.None) PrioritizePlayers.prioritizePlayersList.Add(voidlingIndex);

            voidlingIndex = BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyPhase1");
            if (voidlingIndex != BodyIndex.None) PrioritizePlayers.prioritizePlayersList.Add(voidlingIndex);

            voidlingIndex = BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyPhase2");
            if (voidlingIndex != BodyIndex.None) PrioritizePlayers.prioritizePlayersList.Add(voidlingIndex);

            voidlingIndex = BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyPhase3");
            if (voidlingIndex != BodyIndex.None) PrioritizePlayers.prioritizePlayersList.Add(voidlingIndex);
        }
    }
}
