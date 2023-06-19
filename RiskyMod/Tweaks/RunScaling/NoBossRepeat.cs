using RoR2;
using System;
using System.Collections.Generic;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using System.Linq;

namespace RiskyMod.Tweaks.RunScaling
{
    public class NoBossRepeat
    {
        public static bool enabled = true;
        public static int maxStages = 2;	//Set to 2 so that theres always 1 boss left (since most stages have 3 boss choices)

        public static List<GameObject> previousCards = new List<GameObject>();
        public NoBossRepeat()
        {
            if (!enabled) return;

            RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;

            //Copied from DNSpy
            IL.RoR2.CombatDirector.SetNextSpawnAsBoss += (il) =>
            {
                bool error = true;

                //Mark already-used cards as unavailable.
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCallvirt<DirectorCard>("IsAvailable")
                    ))
                {
                    c.Emit(OpCodes.Ldloc_3);//WeightedSelection<DirectorCard>.ChoiceInfo
                    c.EmitDelegate<Func<bool, WeightedSelection<DirectorCard>.ChoiceInfo, bool>>((flag, choice) =>
                    {
                        if(flag)
                        {
                            if (choice.value != null && choice.value.spawnCard != null)
                            {
                                if (previousCards.Contains(choice.value.spawnCard.prefab))
                                {
                                    return false;
                                }
                            }
                        }
                        return flag;
                    });

                    
                    //Add selected cards to the list
                    if (c.TryGotoNext(
                        x => x.MatchCall<CombatDirector>("PrepareNewMonsterWave")
                        ))
                    {
                        c.EmitDelegate<Func<DirectorCard, DirectorCard>>(card =>
                        {
                            if (card != null && card.spawnCard != null)
                            {
                                if (previousCards.Count > 0 && previousCards.Count + 1 > maxStages)
                                {
                                    previousCards.RemoveAt(0);
                                }
                                previousCards.Add(card.spawnCard.prefab);
                            }
                            return card;
                        });
                        error = false;
                    }
                }
                
                if (error)
                {
                    Debug.LogError("RiskyMod: NoBossRepeat IL Hook failed");
                }
            };
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            previousCards.Clear();
        }
    }
}
