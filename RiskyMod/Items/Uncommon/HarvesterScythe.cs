using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.SharedHooks;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class HarvesterScythe
    {
        public static bool enabled = true;
        public static BuffDef scytheBuff;
        //public static GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/MedkitHealEffect"); //same sfx as "Play_item_proc_crit_heal"
        public HarvesterScythe()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

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

            scytheBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_ScytheBuff",
                true,
                false,
                false,
                new Color(210f / 255f, 50f / 255f, 22f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CrocoRegen").iconSprite
                );

            AssistManager.HandleAssistInventoryActions += OnKillEffect;
            RecalculateStatsAPI.GetStatCoefficients += HandleStats;
            TakeDamage.ModifyInitialDamageActions += ForceDamageCrit;

            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "LifeSteal")
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
                {
                    return hasBuff || self.HasBuff(HarvesterScythe.scytheBuff);
                });
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.HealOnCrit);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.HealOnCrit);
            SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.HealOnCrit, ItemTag.OnKillEffect);
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffCount = sender.GetBuffCount(HarvesterScythe.scytheBuff);
            if (buffCount > 0)
            {
                args.critAdd += 100f;
                args.baseRegenAdd += buffCount * (4f + 0.8f * (sender.level - 1f));
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

        private void ForceDamageCrit(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (!damageInfo.crit && attackerBody.HasBuff(HarvesterScythe.scytheBuff))
            {
                damageInfo.crit = true;
            }
        }
    }
}
