using R2API;
using System.Collections.Generic;

namespace RiskyMod.Enemies.Spawnpools
{
    public class TitanicPlains
    {
        public static bool enabled = true;
        public TitanicPlains()
        {
            if (!enabled) return;

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.Jellyfish, DirectorAPI.Stage.TitanicPlains);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.Jellyfish, DirectorAPI.Stage.TitanicPlainsSimulacrum);

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.AlphaConstruct, DirectorAPI.Stage.TitanicPlains);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.AlphaConstruct, DirectorAPI.Stage.TitanicPlainsSimulacrum);

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.XiConstruct, DirectorAPI.Stage.TitanicPlains);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.XiConstruct, DirectorAPI.Stage.TitanicPlainsSimulacrum);

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.BisonLoop, false, DirectorAPI.Stage.TitanicPlains);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.Bison, false, DirectorAPI.Stage.TitanicPlainsSimulacrum);
        }
    }
}
