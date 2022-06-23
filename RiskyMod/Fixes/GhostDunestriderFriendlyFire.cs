using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Fixes
{
    public class GhostDunestriderFriendlyFire
    {
        public static bool enabled = true;
        public GhostDunestriderFriendlyFire()
        {
            if (!enabled) return;
            IL.EntityStates.ClayBoss.Recover.FireTethers += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<BullseyeSearch, EntityStates.ClayBoss.Recover, BullseyeSearch>>((search, self) =>
                    {
                        if (self.GetTeam() == TeamIndex.Player)
                        {
                            search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                        }
                        return search;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: GhostDunestriderFriendlyFire IL Hook failed");
                }
            };
        }
    }
}
