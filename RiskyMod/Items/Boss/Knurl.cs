using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Boss
{
    public class Knurl
    {
        public static bool enabled = true;
        public Knurl()
        {
            if (!enabled) return;

            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Knurl);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Knurl);

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

            GetStatsCoefficient.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int knurlCount = sender.inventory.GetItemCount(RoR2Content.Items.Knurl);
            if (knurlCount > 0)
            {
                args.healthMultAdd += 0.08f * knurlCount;
                args.armorAdd += 10f * knurlCount;
                args.baseRegenAdd += (1.6f + 0.32f * (sender.level - 1f)) * knurlCount;
            }
        }
    }
}
