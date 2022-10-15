using System;
using MonoMod.Cil;
using UnityEngine;
using RoR2;

namespace RiskyMod.Allies
{
    public class CheaperRepairs
    {
        public static bool enabled = true;
        public CheaperRepairs()
        {
            if (!enabled) return;

			IL.EntityStates.Drone.DeathState.OnImpactServer += (il) =>
			{
				ILCursor c = new ILCursor(il);
				if (c.TryGotoNext(MoveType.After,
					 x => x.MatchCallvirt<RoR2.Run>("GetDifficultyScaledCost")
					 ))
				{
					c.EmitDelegate<Func<int, int>>(cost => Mathf.CeilToInt(cost * 0.5f));
				}
				else
				{
					Debug.LogError("RiskyMod: CheaperRepairs IL Hook failed");
				}
			};
        }
    }
}
