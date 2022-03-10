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
            ItemsCore.ModifyItemDefActions += ModifyItem;

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

            GetStatsCoefficient.HandleStatsInventoryActions += HandleStatsInventory;
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.SprintArmor);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.SprintArmor);

            SneedUtils.SneedUtils.RemoveItemTag(RoR2Content.Items.SprintArmor, ItemTag.SprintRelated);
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int bucklerCount = sender.inventory.GetItemCount(RoR2Content.Items.SprintArmor);
            if (bucklerCount > 0)
            {
                args.armorAdd += 10f * bucklerCount * (sender.isSprinting ? 2f : 1f);
            }
        }
    }
}
