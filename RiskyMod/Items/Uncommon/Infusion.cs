using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RoR2.Orbs;

namespace RiskyMod.Items.Uncommon
{
    public class Infusion
    {
        public static bool enabled = true;
        public Infusion()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Infusion);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Infusion);

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            //LanguageAPI.Add("ITEM_INFUSION_PICKUP", "Killing an enemy permanently increases your maximum health, up to 150.");
            //LanguageAPI.Add("ITEM_INFUSION_DESC", "Killing an enemy increases your <style=cIsHealing>health permanently</style> by <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style>, up to a <style=cIsHealing>maximum</style> of <style=cIsHealing>150 <style=cStack>(+150 per stack)</style> health</style>.");

            AssistManager.HandleAssistActions += OnKillEffect;
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.Infusion);
            if (itemCount > 0)
            {
                int maxInfusionBonus = itemCount * 150;
                if ((ulong)attackerInventory.infusionBonus < (ulong)((long)maxInfusionBonus))
                {
                    InfusionOrb infusionOrb = new InfusionOrb();
                    infusionOrb.origin = victimBody.corePosition;
                    infusionOrb.target = Util.FindBodyMainHurtBox(attackerBody);
                    infusionOrb.maxHpValue = itemCount;
                    OrbManager.instance.AddOrb(infusionOrb);
                }
            }
        }
    }
}
