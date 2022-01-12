using RoR2;
using UnityEngine;
using R2API;
using System.Collections.Generic;

namespace RiskyMod.Enemies.Spawnpools
{
    public class SirensCall
    {
        public static bool enabled = true;
        public SirensCall()
        {

            if (!enabled) return;

            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (stage.stage == DirectorAPI.Stage.SirensCall)
                {
                    List<DirectorAPI.DirectorCardHolder> toRemove = new List<DirectorAPI.DirectorCardHolder>();
                    foreach (DirectorAPI.DirectorCardHolder dch in list)
                    {
                        if (dch.Card.spawnCard == SpawnCards.Beetle)
                        {
                            toRemove.Add(dch);
                        }
                    }

                    foreach (DirectorAPI.DirectorCardHolder dch in toRemove)
                    {
                        list.Remove(dch);
                    }

                    list.Add(DirectorCards.Imp);
                    list.Add(DirectorCards.Reminder);
                }
            };
        }
    }
}
