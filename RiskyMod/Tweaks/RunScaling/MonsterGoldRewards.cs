using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.RunScaling
{
    public class MonsterGoldRewards
    {
        public static bool enabled = true;
		public static bool scaleToInitialDifficulty = true;
		public static bool scaleToInflation = true;
		public MonsterGoldRewards()
        {
            if (!enabled) return;

			On.RoR2.CharacterBody.Start += (orig, self) =>
			{
				orig(self);

				if (Run.instance.gameModeIndex != RiskyMod.simulacrumIndex)
                {
					DeathRewards dw = self.GetComponent<DeathRewards>();
					if (dw && dw.goldReward > 0)
					{
						float chestRatio = (scaleToInitialDifficulty && Stage.instance) ? (Stage.instance.entryDifficultyCoefficient / Run.instance.difficultyCoefficient) : 1f;
						float inflationRatio = scaleToInflation ? 1.4f / (1f + 0.4f * Run.instance.difficultyCoefficient) : 1f; //Couldn't find actual code, but wiki claims Combat Director spawning crerdits gets multiplied by this.

						int goldRewardRaw = (int)Mathf.Max(Mathf.Round(dw.goldReward * chestRatio * inflationRatio), 1f);
						dw.goldReward = (uint)goldRewardRaw;
					}
				}
			};
		}
    }
}
