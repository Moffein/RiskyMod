using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks
{
    public class ScaleCostWithPlayerCount
    {
        public static bool scaleMountainShrine = true;
        public static bool scaleCombatShrine = true;

        public static float extraCostMultiplierPerPlayer = 0.5f;

        private static InteractableSpawnCard shrineCombat = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombat.asset").WaitForCompletion();
        private static InteractableSpawnCard shrineCombatSandy = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombatSandy.asset").WaitForCompletion();
        private static InteractableSpawnCard shrineCombatSnowy = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombatSnowy.asset").WaitForCompletion();

        private static InteractableSpawnCard shrineBoss = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion();
        private static InteractableSpawnCard shrineBossSandy = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBossSandy.asset").WaitForCompletion();
        private static InteractableSpawnCard shrineBossSnowy = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBossSnowy.asset").WaitForCompletion();

        private static int shrineCombatCostInitial;
        private static int shrineBossCostInitial;

        public ScaleCostWithPlayerCount()
        {
            if (!scaleMountainShrine && !scaleCombatShrine) return;

            RoR2.RoR2Application.onLoad += GetInitialShrineCost;

            On.RoR2.SceneDirector.Start += (orig, self) =>
            {
                if (NetworkServer.active && Run.instance)
                {
                    int playerCount = Mathf.Max(Run.instance.participatingPlayerCount, 1);
                    float costMult = 1f + ScaleCostWithPlayerCount.extraCostMultiplierPerPlayer * (playerCount - 1);
                    if (scaleCombatShrine) SetShrineCombatCost(costMult);
                    if (scaleMountainShrine) SetShrineBossCost(costMult);
                }
                orig(self);
            };
        }

        private void GetInitialShrineCost()
        {
            shrineCombatCostInitial = shrineCombat.directorCreditCost;
            shrineBossCostInitial = shrineBoss.directorCreditCost;
        }

        private static void SetShrineCombatCost(float costMultiplier)
        {
            shrineCombat.directorCreditCost = Mathf.FloorToInt(shrineCombatCostInitial * costMultiplier);
            shrineCombatSandy.directorCreditCost = Mathf.FloorToInt(shrineCombatCostInitial * costMultiplier);
            shrineCombatSnowy.directorCreditCost = Mathf.FloorToInt(shrineCombatCostInitial * costMultiplier);
        }

        private static void SetShrineBossCost(float costMultiplier)
        {
            shrineBoss.directorCreditCost = Mathf.FloorToInt(shrineBossCostInitial * costMultiplier);
            shrineBossSandy.directorCreditCost = Mathf.FloorToInt(shrineBossCostInitial * costMultiplier);
            shrineBossSnowy.directorCreditCost = Mathf.FloorToInt(shrineBossCostInitial * costMultiplier);
        }
    }
}
