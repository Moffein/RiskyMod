using UnityEngine;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Uncommon
{
    public class Berzerker
    {
        public static bool enabled = true;
        public static BuffDef berzerkBuff;
        public Berzerker()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.WarCryOnMultiKill);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.WarCryOnMultiKill);

            //LanguageAPI.Add("ITEM_WARCRYONMULTIKILL_PICKUP", "Enter a frenzy after killing enemies.");
            //LanguageAPI.Add("ITEM_WARCRYONMULTIKILL_DESC", "<style=cIsDamage>Killing enemies</style> sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>6s</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>10%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>20%</style> for each enemy killed. Stacks up to 5 <style=cStack>(+3 per stack)</style> times.");

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.AddMultiKill += (il) =>
             {
                 ILCursor c = new ILCursor(il);
                 c.GotoNext(
                      x => x.MatchLdsfld(typeof(RoR2Content.Items), "WarCryOnMultiKill")
                     );
                 c.Remove();
                 c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
             };

            berzerkBuff = ScriptableObject.CreateInstance<BuffDef>();
            berzerkBuff.buffColor = new Color(210f / 255f, 50f / 255f, 22f / 255f);
            berzerkBuff.canStack = true;
            berzerkBuff.isDebuff = false;
            berzerkBuff.name = "RiskyItemTweaks_BerzerkBuff";
            berzerkBuff.iconSprite = RoR2Content.Buffs.WarCryBuff.iconSprite;
            BuffAPI.Add(new CustomBuff(berzerkBuff));

            AssistManager.HandleAssistActions += OnKillEffect;
            GetStatsCoefficient.HandleStatsActions += HandleStats;
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int berzerkCount = sender.GetBuffCount(Berzerker.berzerkBuff);
            if (berzerkCount > 0)
            {
                args.moveSpeedMultAdd += 0.1f * berzerkCount;
                args.attackSpeedMultAdd += 0.2f * berzerkCount;
            }
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {

            int berzerkCount = attackerInventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
            if (berzerkCount > 0)
            {
                //Need to apply buff this way to prevent the visual from disappearing.
                int newBuffStack = Mathf.Min(attackerBody.GetBuffCount(Berzerker.berzerkBuff) + 1, 2 + 3 * berzerkCount);
                int foundBuffs = 0;
                foreach (CharacterBody.TimedBuff tb in attackerBody.timedBuffs)
                {
                    if (tb.buffIndex == Berzerker.berzerkBuff.buffIndex)
                    {
                        tb.timer = 6f + foundBuffs;
                        foundBuffs++;
                    }
                }
                for (int i = 0; i < newBuffStack - foundBuffs; i++)
                {
                    attackerBody.AddTimedBuff(Berzerker.berzerkBuff, 6f + foundBuffs);
                    foundBuffs++;
                }
            }
        }
    }
}
