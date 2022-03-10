using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Enemies.DLC1
{
    public class VoidInfestor
    {
        public static bool enabled = true;
        public static bool noVoidAllies = true;

        public VoidInfestor()
        {
            if (!enabled) return;

            GameObject enemyObject = LegacyResourcesAPI.Load<GameObject>("");
            /*CharacterBody cb = enemyObject.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 35f;
            cb.levelMaxHealth = 10f;*/    //apparently decimals get rounded when autocalculating level stats

            if (noVoidAllies)
            {
                IL.EntityStates.VoidInfestor.Infest.OnEnter += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")
                        );
                    c.EmitDelegate<Func<BullseyeSearch, BullseyeSearch>>(search =>
                    {
                        search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                        return search;
                    });
                };

                IL.EntityStates.VoidInfestor.Infest.FixedUpdate += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(MoveType.After,
                         x => x.MatchCallvirt<CharacterBody>("get_isPlayerControlled")
                        );
                    c.Emit(OpCodes.Ldloc_3);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((playerControlled, body) =>
                    {
                        return playerControlled || (body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Player);
                    });
                };
            }
        }
    }
}
