using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace Risky_Mod.Items.Boss
{
    public class ChargedPerf
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled || !Risky_Mod.disableProcChains) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "LightningStrikeOnHit")
                    );
                c.GotoNext(
                    x => x.MatchStfld<RoR2.Orbs.GenericDamageOrb>("procCoefficient")
                    );
                c.Index--;
                c.Next.Operand = 0f;
            };

            //Effect handled in SharedHooks.OnHitEnemy
        }
    }
}
