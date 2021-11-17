using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Uncommon
{
    public class RoseBuckler
    {
        public static bool enabled = true;
        public RoseBuckler()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.SprintArmor);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.SprintArmor);

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "SprintArmor")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            //LanguageAPI.Add("ITEM_SPRINTARMOR_PICKUP", "Increase armor. Gain bonus armor while sprinting.");
            //LanguageAPI.Add("ITEM_SPRINTARMOR_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>12</style> <style=cStack>(+12 per stack)</style>. <style=cIsUtility>Sprinting</style> increases TOTAL armor by <style=cIsHealing>+50%</style>.");

            GetStatsCoefficient.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int bucklerCount = sender.inventory.GetItemCount(RoR2Content.Items.SprintArmor);
            if (bucklerCount > 0)
            {
                args.armorAdd += 12f * bucklerCount;
            }
        }
    }
}
