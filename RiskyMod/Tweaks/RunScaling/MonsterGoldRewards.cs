using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.RunScaling
{
    public class MonsterGoldRewards
    {
        public static bool enabled = true;
		public static bool scaleToInitialDifficulty = true;
		public static bool scaleToDirectorMultiplier = true;

		public static bool scaleToDirectorMultiplierStage1 = false;

		public static bool linearize = true;

		public static float inflationCoefficient = 0.3f;

		private static float goldBank = 0f;

		public MonsterGoldRewards()
        {
            if (!enabled) return;

            RoR2.Run.onRunStartGlobal += ClearGoldBank;

			On.RoR2.CharacterBody.Start += (orig, self) =>
			{
				orig(self);

				if (Run.instance.gameModeIndex != RiskyMod.simulacrumIndex)
                {
					DeathRewards dw = self.GetComponent<DeathRewards>();
					if (dw && dw.goldReward > 0)
					{
						float chestRatio = (scaleToInitialDifficulty && Stage.instance) ? (Stage.instance.entryDifficultyCoefficient / Run.instance.difficultyCoefficient) : 1f;
						//chestRatio *= chestRatio;	//Gold is a function of difficultyCoefficient squared	//Enabling this feels too harsh

						float directorRatio = ((scaleToDirectorMultiplier && (scaleToDirectorMultiplierStage1 || Run.instance.stageClearCount > 0))
						? CombatDirectorMultiplier.scaledGoldRatio : 1f);
						

						float trueGold = dw.goldReward * chestRatio * directorRatio;

						if (linearize)
                        {
							float diff = (scaleToInitialDifficulty && Stage.instance) ? Stage.instance.entryDifficultyCoefficient : Run.instance.difficultyCoefficient;
							trueGold *= 1.2f * Mathf.Pow(diff, 0.25f) / (1 + inflationCoefficient * diff);	//1.4 is equilibrium. Set to a lower value to make gold gain slowly increase
                        }

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

        private void ClearGoldBank(Run obj)
		{
			goldBank = 0f;
		}
    }
}
