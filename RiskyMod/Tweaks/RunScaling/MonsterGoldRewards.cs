using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.RunScaling
{
    public class MonsterGoldRewards
    {
        public static bool enabled = true;
		private static int stageChestCost = 25;
		public MonsterGoldRewards()
        {
            if (!enabled) return;


			On.RoR2.Stage.Start += (orig, self) =>
			{
				stageChestCost = Run.instance.GetDifficultyScaledCost(25);
				orig(self);
			};


			On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageReport) =>
			{
				if (Run.instance.gameModeIndex != RiskyMod.simulacrumIndex)
				{
					float chestRatio = stageChestCost / (float)Run.instance.GetDifficultyScaledCost(25);
					int goldRewardRaw = (int)Mathf.Max(Mathf.Round(self.goldReward * chestRatio), 1f);
					self.goldReward = (uint)goldRewardRaw;
				}
				orig(self, damageReport);
			};
		}
    }
}
