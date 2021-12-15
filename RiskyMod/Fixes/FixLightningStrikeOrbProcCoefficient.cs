using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Orbs;
using System;

namespace RiskyMod.Fixes
{
    public class FixLightningStrikeOrbProcCoefficient
    {
        public FixLightningStrikeOrbProcCoefficient()
        {
            IL.RoR2.Orbs.SimpleLightningStrikeOrb.OnArrival += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.Emit(OpCodes.Ldarg_0);    //self
                c.EmitDelegate<Func<BlastAttack, SimpleLightningStrikeOrb, BlastAttack>>((blast, self) =>
                {
                    blast.procCoefficient = self.procCoefficient;
                    return blast;
                });
            };
        }
    }
}
