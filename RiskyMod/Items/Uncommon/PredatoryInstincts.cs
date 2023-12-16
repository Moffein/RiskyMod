using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.Uncommon
{
    public class PredatoryInstincts
    {
        public static bool enabled = true;

        public PredatoryInstincts()
        {
            if (!enabled) return;

            On.RoR2.GlobalEventManager.OnCrit += RefreshPredatoryOnCrit;
        }
        
        private void RefreshPredatoryOnCrit(On.RoR2.GlobalEventManager.orig_OnCrit orig, GlobalEventManager self, CharacterBody body, DamageInfo damageInfo, CharacterMaster master, float procCoefficient, ProcChainMask procChainMask)
        {
            orig(self, body, damageInfo, master, procCoefficient, procChainMask);

            //Just refresh the buff without trying to check all the other associated stuff related to actually proccing the item.
            if (!body) return;
            int buffCount = body.GetBuffCount(RoR2Content.Buffs.AttackSpeedOnCrit);
            if (buffCount <= 0) return;

            int storedBuffs = 0;
            CharacterBody.TimedBuff[] buffsToRefresh = new CharacterBody.TimedBuff[buffCount];

            //Find max and store buffs
            float maxBuffDuration = 0f;
            for (int i = 0; i < body.timedBuffs.Count; i++)
            {
                if (body.timedBuffs[i].buffIndex == RoR2Content.Buffs.AttackSpeedOnCrit.buffIndex)
                {
                    if (maxBuffDuration < body.timedBuffs[i].timer) maxBuffDuration = body.timedBuffs[i].timer;

                    if (storedBuffs < buffsToRefresh.Length)
                    {
                        buffsToRefresh[storedBuffs] = body.timedBuffs[i];
                        storedBuffs++;
                    }
                }
            }

            //Refresh duration
            for (int i = 0; i < buffsToRefresh.Length; i++)
            {
                if (buffsToRefresh[i] != null && buffsToRefresh[i].timer < maxBuffDuration)
                {
                    buffsToRefresh[i].timer = maxBuffDuration;
                }
            }
        }
    }
}
