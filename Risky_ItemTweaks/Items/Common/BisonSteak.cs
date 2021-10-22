using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Risky_ItemTweaks.Items.Common
{
    public class BisonSteak
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "FlatHealth")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Changes handled in SharedHooks.GetStatCoefficients
            LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", "Gain 33 max health.");
            LanguageAPI.Add("ITEM_FLATHEALTH_DESC", "Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>33</style> <style=cStack>(+33 per stack)</style>.");
        }
    }
}
