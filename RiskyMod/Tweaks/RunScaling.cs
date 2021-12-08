using UnityEngine;
using RoR2;
using RiskyMod.Fixes;

namespace RiskyMod.Tweaks
{
    public class RunScaling
    {
		public static bool enabled = true;
		public static bool scaleSpawnsOnly = true; //Enemies get way too tanky in big lobbies. Experiment with only affecting spawnrates?
		public static float rewardMultiplier = 0.85f;
        public RunScaling()
        {
			if (!enabled) return;
			On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
				int playerCount = self.participatingPlayerCount;
				float time = self.GetRunStopwatch() * 0.0166666675f; //Convert stopwatch(seconds) into minutes. Why is this Floored in vanilla, and why does it still move anyways despite that?

				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = scaleSpawnsOnly ? 1f : (0.7f + playerCount * 0.3f);
				float timeFactor = time * 0.1111111111f * difficultyDef.scalingValue;//* Mathf.Pow(playerCount, 0.15f)
				float stageFactor = Mathf.Pow(1.18f, self.stageClearCount / 5);  //Exponential scaling happens on a per-loop basis
				float finalDifficulty = (playerFactor + timeFactor) * stageFactor;
				self.compensatedDifficultyCoefficient = finalDifficulty;
				self.difficultyCoefficient = finalDifficulty;

				self.ambientLevel = Mathf.Min(3f * (finalDifficulty - playerFactor) + 1f, Run.ambientLevelCap);

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
				float playerFactor = scaleSpawnsOnly ? (0.7f + Run.instance.participatingPlayerCount * 0.3f) : 1f;
				float stageFactor = Run.instance.stageClearCount < 4 ? Mathf.Pow(1.1f, Run.instance.stageClearCount) : 1.5f;//Needs cap to prevent game from turning into a slideshow. Uncapping it causes excessive T2 Elite spam.
				difficultyCoefficient *= playerFactor * stageFactor;
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
