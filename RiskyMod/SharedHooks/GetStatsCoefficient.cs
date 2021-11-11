using RoR2;
using R2API;
using RiskyMod.Items.Uncommon;
using RiskyMod.Items.Common;
using RiskyMod.Items.Boss;
using UnityEngine;
using RiskyMod.Items.Lunar;
using RiskyMod.Items.Legendary;

namespace RiskyMod.SharedHooks
{
    public class GetStatsCoefficient
    {
        public delegate void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args);
        public static HandleStats HandleStatsActions = HandleStatsMethod;
        private static void HandleStatsMethod(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) { }

        public delegate void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory);
        public static HandleStatsInventory HandleStatsInventoryActions = HandleStatsInventoryMethod;
        private static void HandleStatsInventoryMethod(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory) { }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            HandleStatsActions.Invoke(sender, args);

            if (sender.inventory)
            {
                HandleStatsInventoryActions.Invoke(sender, args, sender.inventory);
            }
        }
    }
}
