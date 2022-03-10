using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC1.Void
{
    public class SaferSpaces
    {
        public static bool enabled = true;
        public SaferSpaces()
        {
            //if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "BearVoidReady")
                    );
                c.GotoNext(
                     x => x.MatchLdcR4(0.9f)
                    );
                c.Next.Operand = 0.95f;
            };
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.BearVoid);
        }
    }
}
