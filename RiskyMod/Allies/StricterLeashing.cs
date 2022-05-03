using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Allies
{
    public class StricterLeashing
    {
        public static bool enabled = true;
        public static float leashDistSq = 90f * 90f;
        public StricterLeashing()
        {
            if (!enabled) return;
            IL.RoR2.Items.MinionLeashBodyBehavior.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);

                c.GotoNext(
                     x => x.MatchLdcR4(RoR2.Items.MinionLeashBodyBehavior.leashDistSq)
                    );
                c.Next.Operand = leashDistSq;

                c.GotoNext(
                     x => x.MatchLdcR4(RoR2.Items.MinionLeashBodyBehavior.leashDistSq)
                    );
                c.Next.Operand = leashDistSq;
            };
        }
    }
}
