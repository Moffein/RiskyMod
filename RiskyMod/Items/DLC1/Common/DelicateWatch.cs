using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using System;

namespace RiskyMod.Items.DLC1.Common
{
    public class DelicateWatch
    {
        public static bool enabled = true;

        public DelicateWatch()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove Vanilla Effect
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "FragileDamageBonus")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));

                c.GotoNext(MoveType.After,
                     x => x.MatchLdfld<HealthComponent.ItemCounts>("fragileDamageBonus")
                    );
                c.EmitDelegate<Func<int, int>>(val => 0);
            };

            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += (CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory) =>
            {
                if (sender.outOfDanger)
                {
                    int itemCount = inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus);
                    if (itemCount > 0)
                    {
                        args.damageMultAdd += 0.08f * itemCount;
                    }
                }
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC1Content.Items.FragileDamageBonus);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.FragileDamageBonus);
            SneedUtils.SneedUtils.RemoveItemTag(DLC1Content.Items.FragileDamageBonus, ItemTag.LowHealth);
        }
    }
}
