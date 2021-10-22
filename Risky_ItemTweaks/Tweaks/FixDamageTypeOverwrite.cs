using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace Risky_ItemTweaks.Tweaks
{
    class FixDamageTypeOverwrite
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixRed")
                    );
                c.Index += 7;
                c.EmitDelegate<Func<bool, bool>>((cond) =>
                {
                    return false;
                });

                //Disable Death Mark - Calculation has been moved to end of SharedHooks.OnHitEnemy
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "DeathMark")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Effect is handled in SharedHooks.OnHitEnemy
        }
    }
}
