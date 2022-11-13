using System;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class BetterProjectileTracking
    {
        public static bool enabled = true;
        public BetterProjectileTracking()
        {
            if (!enabled) return;

            IL.RoR2.Projectile.ProjectileDirectionalTargetFinder.SearchForTarget += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")
                    ))
                {
                    c.EmitDelegate<Func<BullseyeSearch, BullseyeSearch>>((search) =>
                    {
                        search.sortMode = BullseyeSearch.SortMode.Angle;
                        return search;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: BetterProjectileTracking IL Hook failed");
                }
            };
        }
    }
}
