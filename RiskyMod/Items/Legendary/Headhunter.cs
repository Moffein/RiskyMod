using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Legendary
{
    public class HeadHunter
    {
        public static bool enabled = true;
        public static BuffDef headhunterBuff;
        public HeadHunter()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.HeadHunter);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.HeadHunter);

            headhunterBuff = ScriptableObject.CreateInstance<BuffDef>();
            headhunterBuff.buffColor = new Color(210f/255f, 50f/255f, 22f/255f);
            headhunterBuff.canStack = false;
            headhunterBuff.isDebuff = false;
            headhunterBuff.name = "RiskyItemTweaks_HeadhunterBuff";
            headhunterBuff.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffAttackSpeedOnCritIcon");
            BuffAPI.Add(new CustomBuff(headhunterBuff));

            //Buff application handled via Assist Manager

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
            GetStatsCoefficient.HandleStatsActions += HandleStats;
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

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(HeadHunter.headhunterBuff))
            {
                args.moveSpeedMultAdd += 0.5f;
                args.damageMultAdd += 0.3f;
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
                            //attackerBody.AddTimedBuff(HeadHunter.headhunterBuff.buffIndex, duration);
                        }
                    }
                }
            }
        }
    }
}
