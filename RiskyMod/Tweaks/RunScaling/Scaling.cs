using UnityEngine;
using RoR2;
using RiskyMod.Fixes;

namespace RiskyMod.Tweaks.RunScaling
{
    public class Scaling
    {
		public static bool enabled = true;
		public static float rewardMultiplier = 0.85f;

		public static GameModeIndex classicRunIndex;
		public static GameModeIndex simulacrumIndex;

		private static bool isBossStage = false;

        public Scaling()
        {
			On.RoR2.GameModeCatalog.LoadGameModes += (orig) =>
			{
				orig();
				simulacrumIndex = GameModeCatalog.FindGameModeIndex("InfiniteTowerRun");
				classicRunIndex = GameModeCatalog.FindGameModeIndex("ClassicRun");
			};

			if (!enabled) return;

			/*On.RoR2.Stage.Start += (orig, self) =>
			{
				Scaling.isBossStage = false;
				SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
				if (sd)
				{
					if (sd.baseSceneName == "moon" || sd.baseSceneName == "moon2" || sd.baseSceneName == "voidraid")
                    {
						Scaling.isBossStage = true;
					}
				}

				orig(self);
			};*/

			On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
				int playerCount = self.participatingPlayerCount;
				float time = self.GetRunStopwatch() * 0.0166666675f; //Convert stopwatch(seconds) into minutes. Why is this Floored in vanilla, and why does it still move anyways despite that?

				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = 0.7f + playerCount * 0.3f;
				float timeFactor = time * 0.1111111111f * difficultyDef.scalingValue;//* Mathf.Pow(playerCount, 0.15f)
				//float stageFactor = Mathf.Pow(1.18f, self.stageClearCount / 5);  //Exponential scaling happens on a per-loop basis
				int stagesCleared = self.stageClearCount;
				/*if (Scaling.isBossStage && stagesCleared > 0)
                {
					stagesCleared--;
                }*/
				float stageFactor = 1f + 0.5f * Mathf.Floor(stagesCleared / 5);	//Explicitly use Floor to be safe, even though int division already floors it.
				float finalDifficulty = (playerFactor + timeFactor) * stageFactor;
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

			//This wasn't supposed to be enabled.
			/*On.RoR2.InfiniteTowerRun.RecalculateDifficultyCoefficentInternal += (orig, self) =>
			{
				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
				float num = 1.5f * (float)self.waveIndex;
				float num2 = 0.1111111111f * difficultyDef.scalingValue;	//0.0506f, see equivalent
				float num3 = Mathf.Pow(1.01f, (float)self.waveIndex);	//1.02f, needs further investigation but the graph looks about right (higher linear scaling, lower exponential scaling)
				self.difficultyCoefficient = (1f + num2 * num) * num3;
				self.compensatedDifficultyCoefficient = self.difficultyCoefficient;
				self.ambientLevel = Mathf.Min((self.difficultyCoefficient - 1f) * 3f + 1f, 9999f);	//changed from division operation to multiplication, see equivalent
				int ambientLevelFloor = self.ambientLevelFloor;
				self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
				if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
				{
					self.OnAmbientLevelUp();
				}
			};*/

			/*On.RoR2.CombatDirector.DirectorMoneyWave.Update += (orig, self, deltaTime, difficultyCoefficient) =>
			{
				if (Run.instance.gameModeIndex != simulacrumIndex)
				{
					//Needs cap to prevent game from turning into a slideshow. Uncapping it causes excessive T2 Elite spam.
					float stageFactor = Run.instance.stageClearCount < 4 ? Mathf.Pow(1.1f, Run.instance.stageClearCount) : 1.5f;
					difficultyCoefficient *= stageFactor;
				}
				return orig(self, deltaTime, difficultyCoefficient);
			};*/

			On.RoR2.CombatDirector.Awake += (orig, self) =>
			{
				if (Run.instance.gameModeIndex != simulacrumIndex)
				{
					self.creditMultiplier *= 1.2f;
				}
				orig(self);
			};

			On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageReport) =>
			{
				if (Run.instance.gameModeIndex != simulacrumIndex)
				{
					//self.goldReward = (uint)Mathf.CeilToInt(Mathf.Pow(self.goldReward, 0.9f) / Mathf.Pow(1.2f, Run.instance.stageClearCount / 5));
					self.goldReward = (uint)Mathf.CeilToInt(self.goldReward * 0.8333333333f);
				}
				orig(self, damageReport);
			};
		}

		private float GetScaledReward()
		{
			return 1.1f * Run.instance.stageClearCount < 4 ? Mathf.Pow(1.1f, Run.instance.stageClearCount) : 1.5f;
		}
	}
}
