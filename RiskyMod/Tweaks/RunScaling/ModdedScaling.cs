using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.RunScaling
{
    public class ModdedScaling
    {
        public static bool enabled = true;

        public ModdedScaling()
        {
            if (!enabled) return;
            if (LinearScaling.enabled)
            {
                new LinearScaling();
                return;
            }

            //This changes the per-player exponential
            IL.RoR2.Run.RecalculateDifficultyCoefficentInternal += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(0.2f)
                    );
                c.Next.Operand = 0.15f;
            };
		}
    }
}
