using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Survivors.Croco
{
    public class RemovePoisonDamageCap
    {
        public static bool enabled = true;
        public RemovePoisonDamageCap()
        {
            if (!enabled) return;
            IL.RoR2.DotController.AddDot += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(50f)
                    );
                c.Index += 2;
                c.EmitDelegate<Func<float, float>>(orig =>
                {
                    return float.MaxValue;
                });
            };
        }
    }
}
