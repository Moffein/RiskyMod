using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace Risky_ItemTweaks.Items.Boss
{
    public class ChargedPerf
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
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "LightningStrikeOnHit")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Effect handled in SharedHooks.OnHitEnemy
        }
    }
}
