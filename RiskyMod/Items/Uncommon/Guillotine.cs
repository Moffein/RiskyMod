using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class Guillotine
    {
        public static bool enabled = true;

		public Guillotine()
		{
			if (!enabled) return;
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.ExecuteLowHealthElite);
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ExecuteLowHealthElite);

			//Remove Vanilla Effect
			IL.RoR2.CharacterBody.OnInventoryChanged += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "ExecuteLowHealthElite")
					);
				c.Remove();
				c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
			};

			ModifyFinalDamage.ModifyFinalDamageActions += GuillotineBonus;
		}
		private static void GuillotineBonus(DamageMult damageMult, DamageInfo damageInfo,
			HealthComponent victimHealth, CharacterBody victimBody,
			CharacterBody attackerBody, Inventory attackerInventory)
		{
			int lopperCount = attackerInventory.GetItemCount(RoR2Content.Items.ExecuteLowHealthElite);
			if (lopperCount > 0)
			{
				if (victimHealth.combinedHealth <= victimHealth.fullCombinedHealth * 0.3f)
				{
					damageMult.damageMult *= 1f + 0.25f * lopperCount;
					damageInfo.damageColorIndex = DamageColorIndex.Item;

					//Lock the visual effect behind proccing attacks to improve performance
					if (damageInfo.procCoefficient > 0f)
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
	}
}
