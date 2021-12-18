using RoR2;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Survivors.Croco
{
    public class IncreaseRegenerativeHeal
    {
        public IncreaseRegenerativeHeal()
        {
            IL.EntityStates.Croco.Slash.OnMeleeHitAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(0.5f)
                    );
                c.Next.Operand = 0.8f;
            };

            IL.EntityStates.Croco.Bite.OnMeleeHitAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(0.5f)
                    );
                c.Next.Operand = 0.8f;
            };
        }
    }
}
