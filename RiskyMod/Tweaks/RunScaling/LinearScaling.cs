using UnityEngine;
using RoR2;
using RiskyMod.Fixes;

namespace RiskyMod.Tweaks.RunScaling
{
    public class LinearScaling
    {
		public static bool enabled = false;
		public static bool swapToExponential = false;

        public LinearScaling()
        {
			if (!enabled) return;
			On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
				int playerCount = self.participatingPlayerCount;
				float time = self.GetRunStopwatch() * 0.0166666675f; //Convert stopwatch(seconds) into minutes. Why is this Floored in vanilla, and why does it still move anyways despite that?

				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = 0.7f + playerCount * 0.3f;
				float playerMult = Mathf.Pow(playerCount, 0.15f);

				float timeFactor = time * 0.1111111111f * difficultyDef.scalingValue * playerMult;
				int stagesCleared = self.stageClearCount;
				int loopCount = Mathf.FloorToInt(stagesCleared / 5);
				float loopFactor = 1f + 0.25f * loopCount;
				float finalDifficulty = (playerFactor + timeFactor) * loopFactor;

				if (swapToExponential)
				{
					float timeFactorExp = time * 0.0506f * difficultyDef.scalingValue * playerMult;
					float finalDifficultyExp = (playerFactor + timeFactorExp) * Mathf.Pow(1.15f, stagesCleared);

					finalDifficulty = Mathf.Max(finalDifficulty, finalDifficultyExp);
                }

				self.compensatedDifficultyCoefficient = finalDifficulty;
				self.difficultyCoefficient = finalDifficulty;

				//Untitled Difficulty Mod overwrites Run.ambientLevelCap
				self.ambientLevel = Mathf.Min(3f * (finalDifficulty - playerFactor) + 1f, RemoveLevelCap.enabled ? RemoveLevelCap.maxLevel : Run.ambientLevelCap);

				//Vanilla code
				int ambientLevelFloor = self.ambientLevelFloor;
				self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
				if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
				{
					self.OnAmbientLevelUp();
				}
			};
		}
	}
}
