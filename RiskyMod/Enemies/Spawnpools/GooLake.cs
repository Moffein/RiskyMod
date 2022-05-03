using R2API;
using System.Collections.Generic;

namespace RiskyMod.Enemies.Spawnpools
{
    public class GooLake
    {
        public static bool enabled = true;
        public GooLake()
        {
            if (!enabled) return;

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.MagmaWormLoop, false, DirectorAPI.Stage.AbandonedAqueduct);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.MagmaWormLoop, false, DirectorAPI.Stage.AbandonedAqueductSimulacrum);

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.ReminderLoop, false, DirectorAPI.Stage.AbandonedAqueduct);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.ReminderLoop, false, DirectorAPI.Stage.AbandonedAqueductSimulacrum);

            /*DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (stage.stage == DirectorAPI.Stage.AbandonedAqueduct)
                {
                    list.Add(DirectorCards.MagmaWormLoop);
                    list.Add(DirectorCards.ReminderLoop);
                }
            };*/
        }
    }
}
