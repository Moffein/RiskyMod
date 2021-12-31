
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;

namespace RiskyMod.Survivors.Loader
{
    public class ModifyZapDamage
    {
        public static float zapDamageCoefficient = 7f / 9f;
        public ModifyZapDamage()
        {
            IL.EntityStates.Loader.SwingZapFist.OnMeleeHitAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(1f)
                    );
                c.Next.Operand = zapDamageCoefficient;
            };
        }
    }
}
