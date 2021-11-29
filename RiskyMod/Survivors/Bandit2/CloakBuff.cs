using MonoMod.Cil;

namespace RiskyMod.Survivors.Bandit2
{
    public class CloakBuff
    {
        public static bool enabled = true;
        public CloakBuff()
        {
            if (!enabled) return;
            IL.RoR2.CharacterAI.BaseAI.Target.GetBullseyePosition += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(2f)
                    );
                c.Next.Operand = 4f;
            };
        }
    }
}
