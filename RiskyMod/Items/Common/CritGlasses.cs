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
                     //x => x.MatchLdsfld(typeof(RoR2Content.Items), "CritGlasses")
                     x => x.MatchLdfld<CharacterBody>("levelCrit")
                    );
                c.Index += 8;
                c.Next.Operand = 7f;
            };
            //Current LanguageAPI.Add("ITEM_CRITGLASSES_DESC", "Your attacks have a <style=cIsDamage>7%</style> <style=cStack>(+7% per stack)</style> chance to '<style=cIsDamage>Critically Strike</style>', dealing <style=cIsDamage>double damage</style>.");
        }
    }
}
