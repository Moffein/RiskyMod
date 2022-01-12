using R2API;
using System.Collections.Generic;

namespace RiskyMod.Enemies.Spawnpools
{
    public class Wetland
    {
        public static bool enabled = true;
        public Wetland()
        {
            if (!enabled) return;

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (stage.stage == DirectorAPI.Stage.WetlandAspect)
                {
                    List<DirectorAPI.DirectorCardHolder> toRemove = new List<DirectorAPI.DirectorCardHolder>();
                    foreach (DirectorAPI.DirectorCardHolder dch in list)
                    {
                        if (dch.Card.spawnCard == SpawnCards.Wisp)
                        {
                            toRemove.Add(dch);
                        }
                    }

                    foreach (DirectorAPI.DirectorCardHolder dch in toRemove)
                    {
                        list.Remove(dch);
                    }
                }
            };
        }
    }
}
