using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.DLC1.Common
{
    public class DelicateWatch
    {
        public static bool enabled = true;
        public static BuffDef WatchIndicatorBuff;

        public DelicateWatch()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            //This is purely cosmetic. The actual damage boost is strictly tied to the Out of Danger stat, rather than the buff itself.
            WatchIndicatorBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_WatchIndicatorBuff",
                false,
                false,
                false,
                new Color(238f/255f, 208f/255f, 186f/255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite //Todo: unique buff icon
                );

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);

                if (self.inventory)
                {
                    if (self.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus) > 0)
                    {
                        bool hasBuff = self.HasBuff(WatchIndicatorBuff);
                        if (hasBuff && !self.outOfDanger)
                        {
                            if (NetworkServer.active) self.RemoveBuff(WatchIndicatorBuff);
                            RoR2.Util.PlaySound("Play_item_proc_delicateWatch_break", self.gameObject);
                        }
                        else if (!hasBuff && self.outOfDanger)
                        {
                            if (NetworkServer.active) self.AddBuff(WatchIndicatorBuff);

                            RoR2.Util.PlaySound("Play_RiskyMod_DelicateWatch_Ready", self.gameObject);
                        }
                    }
                }
            };

            //Remove Vanilla Effect
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "FragileDamageBonus")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: DelicateWatch TakeDamage IL Hook failed");
                }
            };

            IL.RoR2.HealthComponent.UpdateLastHitTime += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if(c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "fragileDamageBonus")
                    ))
                {
                    c.EmitDelegate<Func<int, int>>(val => 0);
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: DelicateWatch UpdateLastHitTime IL Hook failed");
                }
            };

            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += (CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory) =>
            {
                if (sender.outOfDanger)
                {
                    int itemCount = inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus);
                    if (itemCount > 0)
                    {
                        args.damageMultAdd += 0.1f * itemCount;
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
