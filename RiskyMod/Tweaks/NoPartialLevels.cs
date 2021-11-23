using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    public class NoPartialLevels
    {
        public static bool enabled = true;
        public NoPartialLevels()
        {
            if (!enabled) return;
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCall<CharacterBody>("get_level"),
                     x => x.MatchLdloc(2),
                     x => x.MatchConvR4(),
                     x => x.MatchAdd()
                    );
                c.Index++;
                c.EmitDelegate<Func<float, float>>((origLevel) =>
                {
                    return Mathf.Floor(origLevel);
                });
            };
        }
    }
}
