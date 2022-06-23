using MonoMod.Cil;

namespace RiskyMod.Tweaks.CharacterMechanics
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
                if(c.TryGotoNext(
                     x => x.MatchLdcR4(2f)
                    ))
                {
                    c.Next.Operand = 3f;
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: CloakBuff IL Hook failed");
                }
            };
        }
    }
}
