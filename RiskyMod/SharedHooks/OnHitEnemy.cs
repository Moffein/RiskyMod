using RoR2;
using UnityEngine.Networking;
using RiskyMod.Fixes;
using RiskyMod.Survivors.Bandit2;
using R2API;
using RiskyMod.Survivors;

namespace RiskyMod.SharedHooks
{
    public class OnHitEnemy
	{
		public delegate void OnHitNoAttacker(DamageInfo damageInfo, CharacterBody victimBody);
		public static OnHitNoAttacker OnHitNoAttackerActions = HandleOnHitNoAttackerMethod;
		private static void HandleOnHitNoAttackerMethod(DamageInfo damageInfo, CharacterBody victimBody) { }

		public delegate void OnHitAttacker(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody);
		public static OnHitAttacker OnHitAttackerActions = HandleOnHitAttackerMethod;
		private static void HandleOnHitAttackerMethod(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody) { }

		public static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
			CharacterBody attackerBody = null;
			CharacterBody victimBody = null;
			Inventory attackerInventory = null;

			bool validDamage = NetworkServer.active && damageInfo.procCoefficient > 0f && !damageInfo.rejected;
			bool assistsEnabled = AssistManager.initialized && RiskyMod.assistManager;

			if (validDamage)
			{
				if (damageInfo.attacker)
				{
					attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
					victimBody = victim ? victim.GetComponent<CharacterBody>() : null;

					if (FixDamageTypeOverwrite.enabled)
					{
						if ((damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic)
						{
							DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, 4f * damageInfo.procCoefficient, 1f);
						}
						if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic || attackerBody.HasBuff(RoR2Content.Buffs.AffixRed))
						{
							DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.PercentBurn, 4f * damageInfo.procCoefficient, 1f);
						}
					}

					if (attackerBody)
                    {
						attackerInventory = attackerBody.inventory;
						if (attackerInventory)
						{
							if (assistsEnabled)
							{
								RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.assistLength);
							}
						}
					}

					//Run this before triggering on-hit procs so that procs don't kill enemies before this triggers.
					if (assistsEnabled)
                    {
						if (BanditSpecialGracePeriod.enabled)
						{
							if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) > DamageType.Generic)
							{
								RiskyMod.assistManager.AddBanditAssist(attackerBody, victimBody, BanditSpecialGracePeriod.duration, AssistManager.DirectAssistType.ResetCooldowns);
							}
							if ((damageInfo.damageType & DamageType.GiveSkullOnKill) > DamageType.Generic)
							{
								RiskyMod.assistManager.AddBanditAssist(attackerBody, victimBody, BanditSpecialGracePeriod.duration, AssistManager.DirectAssistType.BanditSkull);
							}
						}
						if (damageInfo.HasModdedDamageType(SharedDamageTypes.Heal10OnKill))
                        {
							RiskyMod.assistManager.AddBanditAssist(attackerBody, victimBody, AssistManager.directAssistLength, AssistManager.DirectAssistType.Heal10OnKill);
						}
					}
				}
			}

			orig(self, damageInfo, victim);

            if (validDamage)
            {
				if (victimBody)
                {
					OnHitNoAttackerActions.Invoke(damageInfo, victimBody);

					if (damageInfo.attacker && attackerBody)
					{
						OnHitAttackerActions.Invoke(damageInfo, victimBody, attackerBody);
					}
				}
            }
        }
    }
}
