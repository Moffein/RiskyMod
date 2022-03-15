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
    public class GetStatCoefficients
    {
        public delegate void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory);
        public static HandleStatsInventory HandleStatsInventoryActions;

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory && HandleStatsInventoryActions != null)
            {
                HandleStatsInventoryActions.Invoke(sender, args, sender.inventory);
            }
        }
    }
}
