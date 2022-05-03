using System.Collections.Generic;
using R2API;


namespace RiskyMod.Enemies.Spawnpools
{
    public class RallypointDelta
    {
        public static bool enabled = true;

        public RallypointDelta()
        {
            if (!enabled) return;
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.Imp, DirectorAPI.Stage.RallypointDelta);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.Imp, DirectorAPI.Stage.RallypointDeltaSimulacrum);

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Imp, false, DirectorAPI.Stage.RallypointDelta);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Imp, false, DirectorAPI.Stage.RallypointDeltaSimulacrum);
        }
    }
}
