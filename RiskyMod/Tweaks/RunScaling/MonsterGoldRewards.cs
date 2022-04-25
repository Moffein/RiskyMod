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

		public static float inflationCoefficient = 0.3f;

		private static float goldBank = 0f;

		public MonsterGoldRewards()
        {
            if (!enabled) return;

			On.RoR2.Run.Start += (orig, self) =>
			{
				goldBank = 0f;
				orig(self);
			};

			On.RoR2.CharacterBody.Start += (orig, self) =>
			{
				orig(self);

				if (Run.instance.gameModeIndex != RiskyMod.simulacrumIndex)
                {
					DeathRewards dw = self.GetComponent<DeathRewards>();
					if (dw && dw.goldReward > 0)
					{
						float chestRatio = (scaleToInitialDifficulty && Stage.instance) ? (Stage.instance.entryDifficultyCoefficient / Run.instance.difficultyCoefficient) : 1f;
						float inflationRatio = scaleToInflation ? (1f + inflationCoefficient) / (1f + inflationCoefficient * Run.instance.difficultyCoefficient) : 1f; //Couldn't find actual code, but wiki claims Combat Director spawning crerdits gets multiplied by this.
						float directorRatio = (scaleToDirectorMultiplier ? CombatDirectorMultiplier.scaledGoldRatio : 1f);

						float trueGold = dw.goldReward * chestRatio * inflationRatio * directorRatio;
						float finalGold = Mathf.Floor(trueGold);

						if (finalGold > 0)
						{
							float difference = trueGold - finalGold;
							if (difference > 0f) goldBank += difference;
							if (goldBank >= 1f)
							{
								goldBank -= 1f;
								finalGold += 1f;
							}
						}

						int finalGoldInt = (int)Mathf.Max(finalGold, 1f);
						dw.goldReward = (uint)finalGoldInt;
					}
				}
			};
		}
    }
}
