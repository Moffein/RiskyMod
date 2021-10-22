using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
namespace Risky_ItemTweaks.Items.Legendary
{
    public class MeatHook
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled || !Risky_ItemTweaks.disableProcChains) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BounceNearby")
                    );
                c.GotoNext(
                    x => x.MatchStfld<RoR2.Orbs.BounceOrb>("procCoefficient")
                    );
                c.Index--;
                c.Next.Operand = 0f;
            };

            //Effect handled on SharedHooks.OnHitEnemy
        }
    }
}
