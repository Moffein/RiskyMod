using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RoR2;
namespace RiskyMod.Items.Common
{
    public class CautiousSlug
    {
        public static bool enabled = true;
        public CautiousSlug()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCall<CharacterBody>("get_outOfDanger")
                    );

                c.GotoNext(
                     x => x.MatchLdcR4(3f)
                    );
                c.Next.Operand = 4f;
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.HealWhileSafe);
        }
    }
}
