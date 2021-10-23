using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;

namespace RiskyMod.Items.Boss
{
    public class Knurl
    {
        public static bool enabled = true;
        public Knurl()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Knurl")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            LanguageAPI.Add("ITEM_KNURL_PICKUP", "Boosts health, regeneration, and armor.");
            LanguageAPI.Add("ITEM_KNURL_DESC", "<style=cIsHealing>Increase maximum health</style> by <style=cIsHealing>10%</style> <style=cStack>(+10% per stack)</style>, <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+1.6 hp/s</style> <style=cStack>(+1.6 hp/s per stack)</style>, and <style=cIsHealing>armor</style> by <style=cIsHealing>10</style> <style=cStack>(+10 per stack)</style>.");

            //Effect handled in SharedHooks.GetStatCoefficients
        }
    }
}
