using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.RunScaling
{
    public class MonsterGoldRewards
    {
        public static bool enabled = true;
		public static bool scaleToInitialDifficulty = true;
		public static bool scaleToInflation = true;
		public static bool scaleToDirectorMultiplier = true;
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
						float inflationRatio = scaleToInflation ? 1.3f / (1f + 0.3f * Run.instance.difficultyCoefficient) : 1f; //Couldn't find actual code, but wiki claims Combat Director spawning crerdits gets multiplied by this.
						float directorRatio = (scaleToDirectorMultiplier ? CombatDirectorMultiplier.scaledGoldRatio : 1f);

						float trueGold = dw.goldReward * chestRatio * inflationRatio * directorRatio;
						float flooredGold = Mathf.Floor(trueGold);
						float roundingFactorPercent = 100f * (trueGold - flooredGold);
						if (roundingFactorPercent > 0f)
                        {
							if (Util.CheckRoll(roundingFactorPercent)) flooredGold += 1f;
                        }

						int goldRewardRaw = (int)Mathf.Max(flooredGold, 1f);
						dw.goldReward = (uint)goldRewardRaw;
					}
				}
			};
		}
    }
}
