using System;
using RoR2;
using MonoMod.Cil;
using UnityEngine;

namespace RiskyMod.Items.Legendary.DLC1
{
    public class LaserScope
    {
        public static bool enabled = true;
        public LaserScope()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {

                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     /*x => x.MatchLdcR4(2f),
                     x => x.MatchLdcR4(1f),
                     x => x.MatchLdloc(42),
                     x => x.MatchConvR4(),
                     x => x.MatchMul(),
                     x => x.MatchAdd(),*/
                     x => x.MatchCall<CharacterBody>("set_critMultiplier")
                    );

                /*c.Index-= 5;
                c.Next.Operand = 0.5f;*/
            };
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC1Content.Items.CritDamage);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.CritDamage);
        }
    }
}
