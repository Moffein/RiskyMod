using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Items.Legendary
{
    public class Soulbound
    {
        public static bool enabled = true;
        public Soulbound()
        {
            if (!enabled) return;

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Talisman")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            AssistManager.HandleAssistInventoryActions += OnKillEffect;
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.Talisman);
            if (itemCount > 0)
            {
                attackerInventory.DeductActiveEquipmentCooldown(2f + itemCount * 2f);
            }
        }
    }
}
