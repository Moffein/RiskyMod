using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace RiskyMod.Items.Common
{
    public class CritGlasses
    {
        public static bool enabled = true;

        public CritGlasses()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdfld<CharacterBody>("levelCrit")
                    );

                c.GotoNext(
                     x => x.MatchLdcR4(10f)
                    );
                c.Next.Operand = 7f;
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.CritGlasses);
        }

    }
}
