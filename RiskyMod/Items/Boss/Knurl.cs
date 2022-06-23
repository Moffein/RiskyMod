using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RiskyMod.SharedHooks;
using System;
using UnityEngine;

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
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Knurl")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Knurl IL Hook failed");
                }
            };

            ItemsCore.ModifyItemDefActions += ModifyItem;
            GetStatCoefficients.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private static void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int knurlCount = sender.inventory.GetItemCount(RoR2Content.Items.Knurl);
            if (knurlCount > 0)
            {
                args.healthMultAdd += 0.08f * knurlCount;
                args.armorAdd += 10f * knurlCount;
                args.baseRegenAdd += (1.6f + 0.32f * (sender.level - 1f)) * knurlCount;
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Knurl);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Knurl);
        }
    }
}
