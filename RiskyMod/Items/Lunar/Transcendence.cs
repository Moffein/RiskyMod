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
        public static bool alwaysShieldGate = true;
        public Transcendence()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.CharacterBody.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld<CharacterBody>("outOfDangerStopwatch"), //Isn't there any other way to increase shield recharge delay? This is messing with other items such as red whip, slug, or even the new reworked OSP. -anreol
                     x => x.MatchLdcR4(7f)
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, CharacterBody, float>>((outOfDangerDelay, self) =>
                    {
                        if (self.inventory)
                        {
                            int stackCount = self.inventory.GetItemCount(RoR2Content.Items.ShieldOnly) - 1;
                            if (stackCount > 0)
                            {
                                outOfDangerDelay += 1f * stackCount;
                            }
                        }
                        return outOfDangerDelay;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Transcendence IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.ShieldOnly);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ShieldOnly);
        }
    }
}
