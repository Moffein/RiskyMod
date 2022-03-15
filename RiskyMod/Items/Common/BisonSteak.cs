using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.SharedHooks;
using UnityEngine;

namespace RiskyMod.Items.Common
{
    public class BisonSteak
    {
        public static bool enabled = true;

        public BisonSteak()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

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
            GetStatCoefficients.HandleStatsInventoryActions += HandleStatsInventory;

        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.FlatHealth);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.FlatHealth);
            SneedUtils.SneedUtils.RemoveItemTag(RoR2Content.Items.FlatHealth, ItemTag.OnKillEffect);
        }

        private static void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int steakCount = sender.inventory.GetItemCount(RoR2Content.Items.FlatHealth);
            if (steakCount > 0)
            {
                args.baseHealthAdd += sender.levelMaxHealth * steakCount;
            }
        }
    }
}
