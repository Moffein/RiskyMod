using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace Risky_Mod.Items.Lunar
{
    public class ShapedGlass
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove vanilla damage boost - don't touch HP since it's entangled with PermanentCurse
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(2f),
                     x => x.MatchLdloc(26),
                     x => x.MatchConvR4(),
                     x => x.MatchCall("UnityEngine.Mathf", "Pow")
                    );
                c.Next.Operand = 1f;
            };

            //Damage boost is redone in SharedHooks.GetStatCoefficients
        }
    }
}
