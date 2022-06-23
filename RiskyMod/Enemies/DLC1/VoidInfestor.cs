using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.DLC1
{
    public class VoidInfestor
    {
        public static bool enabled = true;
        public static bool noVoidAllies = true;

        public VoidInfestor()
        {
            if (!enabled) return;

            GameObject enemyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/VoidInfestorBody.prefab").WaitForCompletion();
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 30f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;

            if (noVoidAllies)
            {
                IL.EntityStates.VoidInfestor.Infest.OnEnter += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(
                         x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")
                        ))
                    {
                        c.EmitDelegate<Func<BullseyeSearch, BullseyeSearch>>(search =>
                        {
                            search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                            return search;
                        });
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("RiskyMod: VoidInfestor Infest.OnEnter IL Hook failed");
                    }
                };

                IL.EntityStates.VoidInfestor.Infest.FixedUpdate += (il) =>
                {
                    bool error = true;

                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchCallvirt<CharacterBody>("get_isPlayerControlled")
                        ))
                    {
                        c.Emit(OpCodes.Ldloc_3);
                        c.EmitDelegate<Func<bool, CharacterBody, bool>>((playerControlled, body) =>
                        {
                            return playerControlled || (body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Player) || body.isBoss;
                        });

                        //Fix allied Ghost Infestors creating new Void Team monsters
                        if (c.TryGotoNext(
                         x => x.MatchCallvirt<CharacterMaster>("set_teamIndex")
                        ))
                        {
                            c.Emit(OpCodes.Ldarg_0);
                            c.EmitDelegate<Func<TeamIndex, EntityStates.VoidInfestor.Infest, TeamIndex>>((team, self) =>
                            {
                                return self.GetTeam();
                            });

                            if (c.TryGotoNext(
                             x => x.MatchCallvirt<TeamComponent>("set_teamIndex")
                            ))
                            {
                                c.Emit(OpCodes.Ldarg_0);
                                c.EmitDelegate<Func<TeamIndex, EntityStates.VoidInfestor.Infest, TeamIndex>>((team, self) =>
                                {
                                    return self.GetTeam();
                                });
                                error = false;
                            }
                        }
                    }

                    if (error)
                    {
                        UnityEngine.Debug.LogError("RiskyMod: VoidInfestor Infest.FixedUpdate IL Hook failed");
                    }
                };
            }
        }
    }
}
