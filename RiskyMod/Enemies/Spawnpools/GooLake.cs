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
