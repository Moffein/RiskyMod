using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;

namespace RiskyMod.Items.Uncommon
{
    public class Infusion
    {
        public static bool enabled = true;
        public Infusion()
        {
            if (!enabled) return;

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            LanguageAPI.Add("ITEM_INFUSION_PICKUP", "Killing an enemy permanently increases your maximum health, up to 150.");
            LanguageAPI.Add("ITEM_INFUSION_DESC", "Killing an enemy increases your <style=cIsHealing>health permanently</style> by <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style>, up to a <style=cIsHealing>maximum</style> of <style=cIsHealing>150 <style=cStack>(+150 per stack)</style> health</style>.");

            //Handled in AssistManager
        }
    }
}
