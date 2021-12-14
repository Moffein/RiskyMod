using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace RiskyMod.Fixes
{
    public class FixFocusCrystalSelfDamage
    {
        public FixFocusCrystalSelfDamage()
        {
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "NearbyDamageBonus")
                    );
                c.Index += 4;
                c.Emit(OpCodes.Ldarg_0);//victim healthcomponent
                c.Emit(OpCodes.Ldarg_1);//damageInfo
                c.EmitDelegate<Func<int, HealthComponent, DamageInfo, int>>((itemCount, self, damageInfo) =>
                {
                    if (itemCount > 0 && self.body && damageInfo.attacker)
                    {
                        if (self.body.gameObject == damageInfo.attacker)
                        {
                            itemCount = 0;
                        }
                    }
                    return itemCount;
                });
            };
        }
    }
}
