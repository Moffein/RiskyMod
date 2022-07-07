using System.Collections.Generic;
using R2API;

namespace RiskyMod.Enemies.Spawnpools
{
    public class SkyMeadow
    {
        public static bool enabled = true;
        public SkyMeadow()
        {
            if (!enabled) return;

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.AlphaConstruct, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.AlphaConstruct, DirectorAPI.Stage.SkyMeadowSimulacrum);

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.XiConstruct, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.XiConstruct, DirectorAPI.Stage.SkyMeadowSimulacrum);

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.BrassContraption, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.BrassContraption, DirectorAPI.Stage.SkyMeadowSimulacrum);

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.MagmaWorm, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.MagmaWorm, DirectorAPI.Stage.SkyMeadowSimulacrum);

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.OverloadingWorm, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.OverloadingWorm, DirectorAPI.Stage.SkyMeadowSimulacrum);

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Imp, false, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Imp, false, DirectorAPI.Stage.SkyMeadowSimulacrum);

            /*DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.GolemBasic, false, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.GolemBasic, false, DirectorAPI.Stage.SkyMeadowSimulacrum);*/

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.MagmaWorm, false, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.MagmaWorm, false, DirectorAPI.Stage.SkyMeadowSimulacrum);

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Reminder, false, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Reminder, false, DirectorAPI.Stage.SkyMeadowSimulacrum);


            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.LunarGolemSkyMeadow, false, DirectorAPI.Stage.SkyMeadow);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.LunarGolemSkyMeadow, false, DirectorAPI.Stage.SkyMeadowSimulacrum);
        }
    }
}
