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
				int playerCount = self.participatingPlayerCount;
				float time = Mathf.Floor(self.GetRunStopwatch() * 0.0166666675f);    //Convert stopwatch(seconds) into minutes
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = 0.7f + playerCount * 0.3f;
				float timeFactor = time * 0.1111111111f * difficultyDef.scalingValue * Mathf.Pow(playerCount, 0.1f); //was 0.15, want to see if this reduces earlygame bulletsponge in MP a bit. might not be necessary
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
			//Seems to be doing this properly when in big lobbies, which is intended.
			On.RoR2.CombatDirector.DirectorMoneyWave.Update += (orig, self, deltaTime, difficultyCoefficient) =>
			{
				difficultyCoefficient *= Run.instance.stageClearCount < 4 ? Mathf.Pow(1.1f, Run.instance.stageClearCount) : 1.5f;	//Needs cap to prevent game from turning into a slideshow. Uncapping it causes excessive T2 Elite spam.
				return orig(self, deltaTime, difficultyCoefficient);
			};

			On.RoR2.CombatDirector.Awake += (orig, self) =>
			{
				self.creditMultiplier *= 1.1f;
				orig(self);
			};

			On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageReport) =>
			{
				self.goldReward = (uint)Mathf.CeilToInt(Mathf.Pow(self.goldReward, 0.9f) / Mathf.Pow(1.2f, Run.instance.stageClearCount / 5));
				orig(self, damageReport);
			};
		}

		private float GetScaledReward()
		{
			return 1.1f * Run.instance.stageClearCount < 4 ? Mathf.Pow(1.1f, Run.instance.stageClearCount) : 1.5f;
		}
	}
}
