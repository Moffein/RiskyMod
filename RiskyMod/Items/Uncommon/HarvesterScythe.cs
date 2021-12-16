using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Uncommon
{
    public class HarvesterScythe
    {
        public static bool enabled = true;
        public static BuffDef scytheBuff;
        //public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/MedkitHealEffect"); //same sfx as "Play_item_proc_crit_heal"
        public HarvesterScythe()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.HealOnCrit);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.HealOnCrit);

            //Remove Vanilla effect
            IL.RoR2.GlobalEventManager.OnCrit += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "HealOnCrit")
					);
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            scytheBuff = ScriptableObject.CreateInstance<BuffDef>();
            scytheBuff.buffColor = new Color(210f / 255f, 50f / 255f, 22f / 255f);
            scytheBuff.canStack = true;
            scytheBuff.isDebuff = false;
            scytheBuff.name = "RiskyItemTweaks_ScytheBuff";
            scytheBuff.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffRegenBoostIcon");
            BuffAPI.Add(new CustomBuff(scytheBuff));

            //Buff stats handled in SharedHooks.GetStatCoefficients
            //Buff removal handled in SharedHooks.TakeDamage; Add this if the healing ends up being too good.

            //LanguageAPI.Add("ITEM_HEALONCRIT_PICKUP", "Gain health regeneration and guaranteed 'Critical Strikes' on kill.");
            //languageAPI.Add("ITEM_HEALONCRIT_DESC", "Gain <style=cIsDamage>5% critical chance</style>. Killing enemies grants guaranteed '<style=cIsDamage>Critical Strikes</style>' and increases <style=cIsHealing>health regeneration</style> by <style=cIsHealing>5%</style> of your <style=cIsHealing>maximum health</style> for <style=cIsDamage>3s</style> <style=cStack>(+1.5s per stack)</style>.");

            AssistManager.HandleAssistActions += OnKillEffect;
            GetStatsCoefficient.HandleStatsActions += HandleStats;
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffCount = sender.GetBuffCount(HarvesterScythe.scytheBuff);
            if (buffCount > 0)
            {
                args.critAdd += 100f;
                if (sender.healthComponent)
                {
                    args.baseRegenAdd += buffCount * (5f + (sender.level - 1f));
                }
            }
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.HealOnCrit);
            if (itemCount > 0)
            {
                attackerBody.AddTimedBuff(HarvesterScythe.scytheBuff, 2.4f + 1.2f * (itemCount - 1));
            }
        }
    }
}
