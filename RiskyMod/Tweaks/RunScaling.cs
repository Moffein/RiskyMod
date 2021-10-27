using UnityEngine;
using RoR2;
using RiskyMod.Fixes;

namespace RiskyMod.Tweaks
{
    public class RunScaling
    {
		public static bool enabled = true;
        public RunScaling()
        {
			if (!enabled) return;
            On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
				int playerCount = (FixPlayercount.enabled || FixPlayercount.standalonePluginLoaded) ? FixPlayercount.GetConnectedPlayers() : self.participatingPlayerCount;
				float time = Mathf.Floor(self.GetRunStopwatch() * 0.0166666675f);    //Convert stopwatch(seconds) into minutes
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = 0.7f + playerCount * 0.3f;
				float timeFactor = time * 0.1111111111f * difficultyDef.scalingValue;   //Should equate to 1 bar per minute on Monsoon. Note: Vanilla multiplies playerFactor^0.2 here.
				float stageFactor = Mathf.Pow(1.1f, self.stageClearCount / 5);  //Exponential scaling happens on a per-loop basis
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
				difficultyCoefficient *= 1.5f;
				return orig(self, deltaTime, difficultyCoefficient);
			};
        }
    }
}
