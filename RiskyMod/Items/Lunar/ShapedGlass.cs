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

            //Remove vanilla damage boost - don't touch HP since it's entangled with PermanentCurse
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(2f),
                     x => x.MatchLdloc(26),
                     x => x.MatchConvR4(),
                     x => x.MatchCall("UnityEngine.Mathf", "Pow")
                    );
                c.Next.Operand = 1f;
            };

            GetStatsCoefficient.HandleStatsInventoryActions += HandleStatsInventory;
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int glassCount = sender.inventory.GetItemCount(RoR2Content.Items.LunarDagger);
            if (glassCount > 0)
            {
                args.damageMultAdd += glassCount;
            }
        }
    }
}
