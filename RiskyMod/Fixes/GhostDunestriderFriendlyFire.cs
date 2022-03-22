using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Fixes
{
    public class GhostDunestriderFriendlyFire
    {
        public static bool enabled = true;
        public GhostDunestriderFriendlyFire()
        {
            IL.EntityStates.ClayBoss.Recover.FireTethers += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<BullseyeSearch, EntityStates.ClayBoss.Recover, BullseyeSearch>>((search, self) =>
                {
                    if (self.GetTeam() == TeamIndex.Player)
                    {
                        search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                    }
                    return search;
                });
            };
        }
    }
}
