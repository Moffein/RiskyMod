using RiskyMod.Items.Uncommon;
using RiskyMod.Tweaks;
using RiskyMod.Tweaks.CharacterMechanics;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class RecalculateStats
    {
		public delegate void HandleRecalculateStats(CharacterBody self);
		public static HandleRecalculateStats HandleRecalculateStatsActions;

		public delegate void HandleRecalculateStatsInventory(CharacterBody self, Inventory inventory);
		public static HandleRecalculateStatsInventory HandleRecalculateStatsInventoryActions;

		public static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			orig(self);
			if (HandleRecalculateStatsActions != null) HandleRecalculateStatsActions.Invoke(self);
			if (self.inventory && HandleRecalculateStatsInventoryActions != null)
            {
				HandleRecalculateStatsInventoryActions.Invoke(self, self.inventory);
            }
		}
    }
}
