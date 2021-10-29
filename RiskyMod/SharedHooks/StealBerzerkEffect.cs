using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Uncommon;
using RoR2;
using System;

namespace RiskyMod.SharedHooks
{
    public class StealBerzerkEffect
    {
        public StealBerzerkEffect()
        {
            //Display visual when using the custom berzerkBuff
            IL.RoR2.CharacterBody.OnClientBuffsChanged += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "WarCryBuff")
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasWarCry, self) =>
                {
                    return hasWarCry || self.HasBuff(Berzerker.berzerkBuff) || self.HasBuff(Headhunter.headhunterBuff);
                });
            };
        }
    }
}
