using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace Risky_Mod.Tweaks
{
    public class ShieldGating
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;
            //Remove OSP in SharedHooks.RecalculateStats

            //Add Shield Gating
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdloc(7),
                     x => x.MatchLdarg(0),
                     x => x.MatchLdfld<HealthComponent>("shield"),
                     x => x.MatchSub(),
                     x => x.MatchStloc(7),
                     x => x.MatchLdarg(0)
                    ); ;
                c.Index += 4;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, HealthComponent, float>>((remainingDamage, self) =>
                {
                    if (self.body.inventory && self.body.inventory.GetItemCount(RoR2Content.Items.ShieldOnly.itemIndex) == 0)
                    {
                        self.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, 0.3f);
                        return 0f;
                    }
                    return remainingDamage;
                });
            };
        }
    }
}
