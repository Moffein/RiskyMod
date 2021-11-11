using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Common
{
    public class BisonSteak
    {
        public static bool enabled = true;
        public BisonSteak()
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
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", "Gain max health.");
            //LanguageAPI.Add("ITEM_FLATHEALTH_DESC", "Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>30%</style> <style=cStack>(+30% per stack)</style> of your <style=cIsHealing>base maximum health</style>.");
            LanguageAPI.Add("ITEM_FLATHEALTH_DESC", "Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>1 <style=cStack>(+1 per stack)</style> level</style>.");

            GetStatsCoefficient.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int steakCount = sender.inventory.GetItemCount(RoR2Content.Items.FlatHealth);
            if (steakCount > 0)
            {
                args.baseHealthAdd += sender.levelMaxHealth * steakCount;
            }
        }
    }
}
