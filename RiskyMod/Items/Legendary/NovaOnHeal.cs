using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace RiskyMod.Items.Legendary
{
    public class NovaOnHeal
    {
        public static bool enabled = true;
        public NovaOnHeal()
        {
            //Nerf Vengeance damage
            IL.RoR2.HealthComponent.ServerFixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdloc(8),
                     x => x.MatchLdloc(6),
                     x => x.MatchLdloc(7),
                     x => x.MatchMul(),
                     x => x.MatchStfld<RoR2.Orbs.DevilOrb>("damageValue")
                    ))
                {
                    c.Index += 4;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, HealthComponent, float>>((damage, self) =>
                    {
                        if (self.itemCounts.invadingDoppelganger > 0)
                        {
                            damage *= 0.1f;
                        }
                        return damage;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: NovaOnHeal IL Hook failed");
                }
            };
        }
    }
}
