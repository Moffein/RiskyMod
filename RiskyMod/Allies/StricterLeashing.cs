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

                bool error = true;
                if (c.TryGotoNext(
                     x => x.MatchLdcR4(RoR2.Items.MinionLeashBodyBehavior.leashDistSq)
                    ))
                {
                    c.Next.Operand = leashDistSq;
                    if (c.TryGotoNext(
                     x => x.MatchLdcR4(RoR2.Items.MinionLeashBodyBehavior.leashDistSq)
                    ))
                    {
                        c.Next.Operand = leashDistSq;
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: StricterLeashing IL Hook failed");
                }
            };
        }
    }
}
