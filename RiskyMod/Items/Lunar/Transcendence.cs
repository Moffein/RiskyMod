using RoR2;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;

namespace RiskyMod.Items.Lunar
{
    public class Transcendence
    {
        public static bool enabled = true;
        public Transcendence()
        {
            if (!enabled) return;

            IL.RoR2.CharacterBody.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCall<CharacterBody>("UpdateBuffs"),
                     x => x.MatchLdarg(0),
                     x => x.MatchLdfld<CharacterBody>("outOfDangerStopwatch"),
                     x => x.MatchLdcR4(7f)
                    );
                c.Index += 4;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, CharacterBody, float>>((outOfDangerDelay, self) =>
                {
                    if (self.inventory)
                    {
                        outOfDangerDelay += self.inventory.GetItemCount(RoR2Content.Items.ShieldOnly);
                    }
                    return outOfDangerDelay;
                });
            };

            LanguageAPI.Add("ITEM_SHIELDONLY_PICKUP", "Convert all your health into shield. Increase maximum health... <color=#FF7F7F>BUT increase shield regeneration delay.</color>");
            LanguageAPI.Add("ITEM_SHIELDONLY_DESC", "<style=cIsHealing>Convert</style> all but <style=cIsHealing>1 health</style> into <style=cIsHealing>regenerating shields</style>. <style=cIsHealing>Gain 50% <style=cStack>(+25% per stack)</style> maximum health</style>. Increase <style=cIsHealing>shield regeneration delay</style> by <style=cIsHealing>1s</style> <style=cStack>(+1s per stack)</style>.");
        }
    }
}
