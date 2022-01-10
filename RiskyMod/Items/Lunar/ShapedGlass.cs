using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;

namespace RiskyMod.Items.Lunar
{
    public class ShapedGlass
    {
        public static bool enabled = true;
        public ShapedGlass()
        {
            if (!enabled) return;

            //Remove vanilla  effects
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "LunarDagger")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            GetStatsCoefficient.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int glassCount = sender.inventory.GetItemCount(RoR2Content.Items.LunarDagger);
            if (glassCount > 0)
            {
                args.damageMultAdd += glassCount;
                args.baseCurseAdd += glassCount;
            }
        }
    }
}
