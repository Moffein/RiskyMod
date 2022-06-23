using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Items.Common
{
    class TopazBrooch
    {
        public static bool enabled = true;
        public TopazBrooch()
        {
            if (!enabled) return;

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BarrierOnKill")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: TopazBrooch IL Hook failed");
                }
            };

            AssistManager.HandleAssistInventoryActions += OnKillEffect;
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.BarrierOnKill);
            if (itemCount > 0)
            {
                attackerBody.healthComponent.AddBarrier(15f * itemCount);
            }
        }
    }
}
