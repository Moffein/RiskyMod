using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace RiskyMod.Items.Common
{
    public class CritGlasses
    {
        public static bool enabled = true;
        public static ItemDef itemDef = RoR2Content.Items.CritGlasses;

        public CritGlasses()
        {
            if (!enabled) return;
            //HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, itemDef);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, itemDef);

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
    }
}
