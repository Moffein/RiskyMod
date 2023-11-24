using UnityEngine;
using RoR2;
using RiskyMod.Fixes;

namespace RiskyMod.Tweaks.RunScaling
{
    public class LinearScaling
    {
		public static bool enabled = false;
		public static bool swapToNormalScaling = false;

        public LinearScaling()
        {
			if (!enabled) return;
            On.RoR2.Run.RecalculateDifficultyCoefficentInternal += Run_RecalculateDifficultyCoefficentInternal2;
			On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
				float vanillaDiff = self.difficultyCoefficient;

				int playerCount = self.participatingPlayerCount;
				float time = self.GetRunStopwatch() * 0.016666668f; //Convert stopwatch(seconds) into minutes. Why is this Floored in vanilla, and why does it still move anyways despite that?

				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float initialPlayerFactor = 0.7f + playerCount * 0.3f;
				float playerExponential = Mathf.Pow((float)self.participatingPlayerCount, ModdedScaling.enabled ? 0.15f : 0.2f);
                int stagesCleared = self.stageClearCount;

                float timeFactorLinear = time * 0.1111111111f * difficultyDef.scalingValue * playerExponential;
				int loopCount = Mathf.FloorToInt(stagesCleared / 5);
				float loopFactor = 1f + 0.25f * loopCount;
				float finalDifficultyLinear = (initialPlayerFactor + timeFactorLinear) * loopFactor;

				if (swapToNormalScaling)
                {
                    float timeFactorNormal = time * 0.0506f * difficultyDef.scalingValue * playerExponential;
                    float stageExponential = Mathf.Pow(1.15f, (float)self.stageClearCount);
                    float finalDifficultyNormal = (initialPlayerFactor + timeFactorNormal) * stageExponential;

                    if (finalDifficultyNormal > finalDifficultyLinear)
                    {
                        finalDifficultyLinear = finalDifficultyNormal;
                    }
                }

				self.compensatedDifficultyCoefficient = finalDifficultyLinear;
				self.difficultyCoefficient = finalDifficultyLinear;

				//Untitled Difficulty Mod overwrites Run.ambientLevelCap
				self.ambientLevel = Mathf.Min(3f * (finalDifficultyLinear - initialPlayerFactor) + 1f, RemoveLevelCap.enabled ? RemoveLevelCap.maxLevel : Run.ambientLevelCap);

				//Vanilla code
				int ambientLevelFloor = self.ambientLevelFloor;
				self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
				if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
				{
					self.OnAmbientLevelUp();
				}
			};
		}

        private void Run_RecalculateDifficultyCoefficentInternal2(On.RoR2.Run.orig_RecalculateDifficultyCoefficentInternal orig, Run self)
        {
            float currentTime = self.GetRunStopwatch();
            DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
            float minutes = Mathf.Floor(currentTime * 0.016666668f);
            float initialPlayerFactor = 0.7f + (float)self.participatingPlayerCount * 0.3f;
            float playerExponential = Mathf.Pow((float)self.participatingPlayerCount, 0.2f);
            float timeFactor = 0.0506f * difficultyDef.scalingValue * playerExponential;
            float stageExponential = Mathf.Pow(1.15f, (float)self.stageClearCount);

            self.compensatedDifficultyCoefficient = (initialPlayerFactor + timeFactor * minutes) * stageExponential;
            self.difficultyCoefficient = (initialPlayerFactor + timeFactor * minutes) * stageExponential;

            float num10 = (initialPlayerFactor + timeFactor * (currentTime * 0.016666668f)) * Mathf.Pow(1.15f, (float)self.stageClearCount);
            self.ambientLevel = Mathf.Min((num10 - initialPlayerFactor) / 0.33f + 1f, (float)Run.ambientLevelCap);
            int ambientLevelFloor = self.ambientLevelFloor;
            self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
            if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
            {
                self.OnAmbientLevelUp();
            }
        }
    }
}
