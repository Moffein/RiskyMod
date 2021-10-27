using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Tweaks
{
    public class ShieldGating
    {
        public static bool enabled = true;
        public ShieldGating()
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
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((remainingDamage, self, damageInfo) =>
                {
                    if (!((damageInfo.damageType & DamageType.BypassArmor) > DamageType.Generic) && self.body.inventory)
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
