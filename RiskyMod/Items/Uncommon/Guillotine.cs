﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class Guillotine
    {
        public static bool enabled = true;
		public static bool reduceVFX = true;

		public Guillotine()
		{
			if (!enabled) return;
			ItemsCore.ModifyItemDefActions += ModifyItem;

			//Remove Vanilla Effect
			IL.RoR2.CharacterBody.OnInventoryChanged += (il) =>
			{
				ILCursor c = new ILCursor(il);
				if(c.TryGotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "ExecuteLowHealthElite")
					))
				{
					c.Remove();
					c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
				}
				else
				{
					UnityEngine.Debug.LogError("RiskyMod: Guillotine IL Hook failed");
				}
			};

			if (SoftDependencies.LinearDamageLoaded)
            {
                SneedHooks.ModifyFinalDamage.ModifyFinalDamageAttackerActions += ModifyFinalDamage_Additive;
            }
			else
            {
                SneedHooks.ModifyFinalDamage.ModifyFinalDamageAttackerActions += ModifyFinalDamage;
            }
		}

        private void ModifyFinalDamage(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (!attackerBody.inventory || !victimBody.healthComponent) return;
            int lopperCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.ExecuteLowHealthElite);
            if (lopperCount <= 0) return;
            HealthComponent victimHealth = victimBody.healthComponent;

            if (victimHealth.combinedHealth <= victimHealth.fullCombinedHealth * 0.5f)
            {
                damageModifierArgs.damageMultFinal *= 1f + (0.3f * lopperCount);
                if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                {
                    damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                }

                //Lock the visual effect behind proccing attacks to improve performance
                if (damageInfo.procCoefficient > 0f)
                {
                    if (!reduceVFX || UnityEngine.Random.Range(0f, 100f) <= 100f * damageInfo.damage / (victimHealth.fullCombinedHealth * 0.3f * 0.1f))
                    {
                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.executeEffectPrefab, new EffectData
                        {
                            origin = victimBody.corePosition,
                            scale = victimBody.radius * 0.3f * damageInfo.procCoefficient   //not sure if radius is even getting affected
                        }, true);
                    }
                }
            }
        }
        private void ModifyFinalDamage_Additive(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (!attackerBody.inventory || !victimBody.healthComponent) return;
            int lopperCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.ExecuteLowHealthElite);
            if (lopperCount <= 0) return;
            HealthComponent victimHealth = victimBody.healthComponent;

            if (victimHealth.combinedHealth <= victimHealth.fullCombinedHealth * 0.5f)
            {
                damageModifierArgs.damageMultAdd += 0.3f * lopperCount;
                if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                {
                    damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                }

                //Lock the visual effect behind proccing attacks to improve performance
                if (damageInfo.procCoefficient > 0f)
                {
                    if (!reduceVFX || UnityEngine.Random.Range(0f, 100f) <= 100f * damageInfo.damage / (victimHealth.fullCombinedHealth * 0.3f * 0.1f))
                    {
                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.executeEffectPrefab, new EffectData
                        {
                            origin = victimBody.corePosition,
                            scale = victimBody.radius * 0.3f * damageInfo.procCoefficient   //not sure if radius is even getting affected
                        }, true);
                    }
                }
            }
        }

        private static void ModifyItem()
		{
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.ExecuteLowHealthElite);
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ExecuteLowHealthElite);
			SneedUtils.SneedUtils.RemoveItemTag(RoR2Content.Items.ExecuteLowHealthElite, ItemTag.AIBlacklist);
		}
	}
}
