using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.SharedHooks;
using System;

namespace RiskyMod.Items.Legendary
{
    public class HeadHunter
    {
        public static bool enabled = true;
        public HeadHunter()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.HeadHunter);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.HeadHunter);

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "HeadHunter")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            AssistManager.HandleAssistActions += OnKillEffect;
            ModifyFinalDamage.ModifyFinalDamageActions += EliteBonus;
        }

        private static void EliteBonus(DamageMult damageMult, DamageInfo damageInfo,
            HealthComponent victimHealth, CharacterBody victimBody,
            CharacterBody attackerBody, Inventory attackerInventory)
        {
            int hhCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
            if (hhCount > 0)
            {
                if (victimBody.isElite)
                {
                    damageMult.damageMult *= 1.3f;
                    damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                }
            }
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            if (victimBody.isElite)
            {
                int hhCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
                if (hhCount > 0)
                {
                    float duration = 5f + 5f * hhCount;
                    for (int l = 0; l < BuffCatalog.eliteBuffIndices.Length; l++)
                    {
                        BuffIndex buffIndex = BuffCatalog.eliteBuffIndices[l];
                        if (victimBody.HasBuff(buffIndex))
                        {
                            attackerBody.AddTimedBuff(buffIndex, duration);
                        }
                    }
                }
            }
        }
    }
}
