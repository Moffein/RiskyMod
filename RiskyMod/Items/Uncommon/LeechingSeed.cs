using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class LeechingSeed
    {
        public static bool enabled = true;
        public LeechingSeed()
        {
			if (!enabled) return;
			ItemsCore.ModifyItemDefActions += ModifyItem;

			//Remove vanilla effect.
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);
				if (c.TryGotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "Seed")
					))
				{
					c.Remove();
					c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
				}
				else
				{
					UnityEngine.Debug.LogError("RiskyMod: LeechingSeed IL Hook failed");
				}
			};

			OnHitEnemy.OnHitAttackerInventoryActions += HealOnHitInitialDamage;
		}
		private static void ModifyItem()
		{
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Seed);
		}

		private static void HealOnHitInitialDamage(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory)
        {
			if (!damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
			{
				int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.Seed);
				if (itemCount > 0)
				{
					float toHeal = 1f + damageInfo.damage * (0.01f + 0.01f * itemCount) * damageInfo.procCoefficient;
					damageInfo.procChainMask.AddProc(ProcType.HealOnHit);
					attackerBody.healthComponent.Heal(toHeal, damageInfo.procChainMask);

				}
			}
        }
	}
}
