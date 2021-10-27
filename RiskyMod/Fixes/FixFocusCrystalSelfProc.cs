using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace RiskyMod.Fixes
{
    public class FixFocusCrystalSelfProc
    {
        public FixFocusCrystalSelfProc()
        {
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "NearbyDamageBonus")
                    );
                c.Index += 5;
                c.Emit(OpCodes.Ldarg_0);//victim healthcomponent
                c.Emit(OpCodes.Ldarg_1);//damageInfo
                c.EmitDelegate<Func<bool, HealthComponent, DamageInfo, bool>>((flag, self, damageInfo) =>
                {
                    if (flag && self.body && damageInfo.attacker)
                    {
                        flag = !(self.body.gameObject == damageInfo.attacker);
                    }
                    return flag;
                });
            };
        }
    }
}
