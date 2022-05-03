using R2API;
using System.Collections.Generic;

namespace RiskyMod.Enemies.Spawnpools
{
    public class StadiaJungle
    {
        public static bool enabled = true;
        public StadiaJungle()
        {
            if (!enabled) return;

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.ClayDunestrider, DirectorAPI.Stage.SunderedGrove);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.MiniMushrum, DirectorAPI.Stage.SunderedGrove);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.Lemurian, DirectorAPI.Stage.SunderedGrove);

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Grovetender, false, DirectorAPI.Stage.SunderedGrove);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Mushrum, false, DirectorAPI.Stage.SunderedGrove);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Vulture, false, DirectorAPI.Stage.SunderedGrove);
        }
    }
}
