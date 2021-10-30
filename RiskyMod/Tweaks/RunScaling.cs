using UnityEngine;
using RoR2;
using RiskyMod.Fixes;

namespace RiskyMod.Tweaks
{
    public class RunScaling
    {
		public static bool enabled = true;
		public static float rewardMultiplier = 0.85f;
        public RunScaling()
        {
			if (!enabled) return;
            On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
				int playerCount = (FixPlayercount.enabled || FixPlayercount.standalonePluginLoaded) ? FixPlayercount.GetConnectedPlayers() : self.participatingPlayerCount;
				float time = Mathf.Floor(self.GetRunStopwatch() * 0.0166666675f);    //Convert stopwatch(seconds) into minutes
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = 0.7f + playerCount * 0.3f;
				float timeFactor = time * 0.1111111111f * difficultyDef.scalingValue * Mathf.Pow(playerCount, 0.2f);
				float stageFactor = Mathf.Pow(1.2f, self.stageClearCount / 5);  //Exponential scaling happens on a per-loop basis
				float finalDifficulty = (playerFactor + timeFactor) * stageFactor;
				self.compensatedDifficultyCoefficient = finalDifficulty;
				self.difficultyCoefficient = finalDifficulty;

				self.ambientLevel = 3f * (finalDifficulty - playerFactor) + 1f;  //use Run.ambientLevelCap to cap max level

				//Vanilla code
				int ambientLevelFloor = self.ambientLevelFloor;
				self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
				if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
				{
					self.OnAmbientLevelUp();
				}
			};

			//Increase director intensity. Goal is to have the same level of craziness as stage 5 in Starstorm, with the constant boss spawns and big monsters everywhere.
			On.RoR2.CombatDirector.DirectorMoneyWave.Update += (orig, self, deltaTime, difficultyCoefficient) =>
			{
				//Notes:
				//1.5f doesn't change much
				//3f increases spawnrates, but mostly elite trash mobs spawn. Players get too much money.
				//5f seems to be too chaotic. Lots of flying enemies on Stage 5, not enough Parents. Still spawns lots of elite trash on early stages.
				//Check the monster card selection stuff later. Goal is to have more heavy mobs spawn earlier.

				//Maybe just do a flat addition to the difficultyCoefficient? - Maybe
				//Multiply by loop count?
				//difficultyCoefficient = (2f * DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty).scalingValue + 1.5f*difficultyCoefficient) * (Run.instance.stageClearCount/5);
				difficultyCoefficient *= Run.instance.stageClearCount < 5 ? Mathf.Pow(1.15f, Run.instance.stageClearCount) : 2.5f;	//Needs cap to prevent game from turning into a slideshow. Will definitely add config to uncap it for those who want it.
				return orig(self, deltaTime, difficultyCoefficient);
			};

			On.RoR2.CombatDirector.Awake += (orig, self) =>
			{
				self.creditMultiplier *= 1.15f;
				self.expRewardCoefficient = self.expRewardCoefficient / GetScaledExpRewardMult();
				orig(self);
			};
		}

		public static float GetScaledExpRewardMult()
		{
			return RunScaling.enabled ? 0.9f * (Run.instance.stageClearCount < 5 ? Mathf.Pow(1.2f, Run.instance.stageClearCount) : 2.5f) : 1f;

		}
	}
}
