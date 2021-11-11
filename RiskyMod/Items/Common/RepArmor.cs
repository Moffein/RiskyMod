using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using System;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Common
{
    public class RepArmor
    {
        public static bool enabled = true;
        public RepArmor()
        {
            if (!enabled) return;

            //Remove vanilla effect
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "armorPlate")
                    );
                c.Index++;
                c.EmitDelegate<Func<int, int>>((itemCount) =>
                {
                    return 0;
                });
            };

            LanguageAPI.Add("ITEM_REPULSIONARMORPLATE_PICKUP", "Increase armor by 5.");
            LanguageAPI.Add("ITEM_REPULSIONARMORPLATE_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>5</style>.");

            GetStatsCoefficient.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int rapCount = sender.inventory.GetItemCount(RoR2Content.Items.ArmorPlate);
            if (rapCount > 0)
            {
                args.armorAdd += 5f * rapCount;
            }
        }
    }
}
