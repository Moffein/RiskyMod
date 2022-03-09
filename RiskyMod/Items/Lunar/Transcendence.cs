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
            On.RoR2.ItemCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.ShieldOnly);
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ShieldOnly);
            };

            IL.RoR2.CharacterBody.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCall<CharacterBody>("UpdateBuffs"),
                     x => x.MatchLdarg(0),
                     x => x.MatchLdfld<CharacterBody>("outOfDangerStopwatch"), //Isn't there any other way to increase shield recharge delay? This is messing with other items such as red whip, slug, or even the new reworked OSP. -anreol
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
        }
    }
}
