using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Items.Legendary
{
    public class Brainstalks
    {
        public static bool enabled = true;
        public Brainstalks()
        {
            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "KillEliteFrenzy")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Brainstalks IL Hook failed");
                }
            };

            AssistManager.HandleAssistInventoryActions += OnKillEffect;
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            if (victimBody.isElite)
            {
                int bsCount = attackerInventory.GetItemCount(RoR2Content.Items.KillEliteFrenzy);
                if (bsCount > 0)
                {
                    attackerBody.AddTimedBuff(RoR2Content.Buffs.NoCooldowns, bsCount * 4f);
                }
            }
        }
    }
}
