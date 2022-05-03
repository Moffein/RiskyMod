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

            //Jellyfish are unique to Distant Roost, Wisps are unique to Titanic Plains.
            /*DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (stage.stage == DirectorAPI.Stage.TitanicPlains)
                {
                    List<DirectorAPI.DirectorCardHolder> toRemove = new List<DirectorAPI.DirectorCardHolder>();
                    foreach (DirectorAPI.DirectorCardHolder dch in list)
                    {
                        if (dch.Card.spawnCard == SpawnCards.Jellyfish)
                        {
                            toRemove.Add(dch);
                        }
                    }

                    foreach (DirectorAPI.DirectorCardHolder dch in toRemove)
                    {
                        list.Remove(dch);
                    }
                }
            };*/
        }
    }
}
