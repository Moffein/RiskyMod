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

            /*DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (stage.stage == DirectorAPI.Stage.SunderedGrove)
                {
                    List<DirectorAPI.DirectorCardHolder> toRemove = new List<DirectorAPI.DirectorCardHolder>();
                    foreach (DirectorAPI.DirectorCardHolder dch in list)
                    {
                        if (dch.Card.spawnCard == SpawnCards.Lemurian
                        || dch.Card.spawnCard == SpawnCards.GreaterWisp || dch.Card.spawnCard == SpawnCards.Golem || dch.Card.spawnCard == SpawnCards.Mushrum   //Minibosses
                        || dch.Card.spawnCard == SpawnCards.TitanBlackBeach || dch.Card.spawnCard == SpawnCards.TitanDampCave
                        || dch.Card.spawnCard == SpawnCards.TitanGolemPlains || dch.Card.spawnCard == SpawnCards.TitanGooLake
                        || dch.Card.spawnCard == SpawnCards.Vagrant)
                        {
                            toRemove.Add(dch);
                        }
                    }

                    foreach (DirectorAPI.DirectorCardHolder dch in toRemove)
                    {
                        list.Remove(dch);
                    }

                    list.Add(DirectorCards.Mushrum);    //Replace Greater Wisp (this is a miniboss instead of a basic monster)

                    list.Add(DirectorCards.Vulture);    //Replace Lemurian

                    list.Add(DirectorCards.Beetle); //Replace Mushrum (mushrums are considered basic monsters)

                    list.Add(DirectorCards.BeetleGuard);    //Replace Golem

                    list.Add(DirectorCards.BeetleQueen);    //Replace Titan, will this even do anything on this map?
                    list.Add(DirectorCards.Grovetender);    //Replace Vagrant. Not sure if Vagrants should be removed since they look nice.
                }
            };*/
        }
    }
}
