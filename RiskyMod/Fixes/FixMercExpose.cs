using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Fixes
{
    //Merged from https://github.com/Moffein/MercExposeFix
    public class FixMercExpose
    {
        public static bool enabled = true;
        public FixMercExpose()
        {
            if (!enabled) return;
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "MercExpose"),
                     x => x.MatchCallvirt<CharacterBody>("RemoveBuff")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyBuffDef));

                //Remove buff when it is confirmed that it's Merc attacking.
                c.GotoNext(
                      x => x.MatchCallvirt<CharacterBody>("get_damage")
                     );
                c.Emit(OpCodes.Ldarg_0);//victim healthcomponent
                c.EmitDelegate<Func<CharacterBody, HealthComponent, CharacterBody>>((body, victimHealth) =>
                {
                    if (victimHealth.body)
                    {
                        victimHealth.body.RemoveBuff(RoR2Content.Buffs.MercExpose);
                    }
                    return body;
                });
            };
        }
    }
}
