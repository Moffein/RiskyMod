using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.RunScaling
{
    public class ModdedScaling
    {
        public static bool enabled = true;

        public ModdedScaling()
        {
            if (!enabled) return;
            if (LinearScaling.enabled)
            {
                new LinearScaling();
                return;
            }

			On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
			{
				int playerCount = self.participatingPlayerCount;
				float time = self.GetRunStopwatch() * 0.0166666675f; //Convert stopwatch(seconds) into minutes. Why is this Floored in vanilla, and why does it still move anyways despite that?

				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
				float initialPlayerFactor = 0.7f + playerCount * 0.3f;
				float playerFactor = Mathf.Pow(playerCount, 0.15f);	//0.2f vanilla
				float timeFactor = time * 0.0506f * difficultyDef.scalingValue * playerFactor;	//0.506 vanilla
				int stagesCleared = self.stageClearCount;

				//float stageFactor = 1f + stagesCleared * 0.2f;	//exp 1.15 in vanilla
				float stageFactor = Mathf.Pow(1.15f, stagesCleared);
				float finalDifficulty = (initialPlayerFactor + timeFactor) * stageFactor;
				self.compensatedDifficultyCoefficient = finalDifficulty;
				self.difficultyCoefficient = finalDifficulty;

				//Untitled Difficulty Mod overwrites Run.ambientLevelCap
				self.ambientLevel = Mathf.Min(3f * (finalDifficulty - initialPlayerFactor) + 1f, RemoveLevelCap.enabled ? RemoveLevelCap.maxLevel : Run.ambientLevelCap);

				//Vanilla code
				int ambientLevelFloor = self.ambientLevelFloor;
				self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
				if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
				{
					self.OnAmbientLevelUp();
				}
			};
			
			//InfiniteTower doesn't use PlayerFactor?
			/*On.RoR2.InfiniteTowerRun.RecalculateDifficultyCoefficentInternal += (orig, self) =>
			{
				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);

				float waveFactor = 1.5f * (float)self.waveIndex;
				float scalingFactor = 0.0506f * difficultyDef.scalingValue;
				float waveExponential = Mathf.Pow(1.02f, (float)self.waveIndex);

				self.difficultyCoefficient = (1f + scalingFactor * waveFactor) * waveExponential;
				self.compensatedDifficultyCoefficient = self.difficultyCoefficient;

				self.ambientLevel = Mathf.Min((self.difficultyCoefficient - 1f) * 3f + 1f, 9999f);  //changed from division operation to multiplication, see equivalent
				int ambientLevelFloor = self.ambientLevelFloor;
				self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
				if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
				{
					self.OnAmbientLevelUp();
				}
			};*/
		}
    }
}
