using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Tweaks
{
    class NoVoidAllies
    {
        public static bool enabled = true;
        public NoVoidAllies()
        {
            if (!enabled) return;

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
