using R2API;
using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.Legendary;
using RiskyMod.Survivors;
using RiskyMod.Survivors.Bandit2;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class OnCharacterDeath
    {
		public delegate void OnCharacterDeathInventory(GlobalEventManager self, DamageReport damageReport, CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody);
		public static OnCharacterDeathInventory OnCharacterDeathInventoryActions;

		public static void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

			if (NetworkServer.active)
            {
				CharacterBody attackerBody = damageReport.attackerBody;
				CharacterMaster attackerMaster = damageReport.attackerMaster;
				//TeamIndex attackerTeamIndex = damageReport.attackerTeamIndex;
				DamageInfo damageInfo = damageReport.damageInfo;
				//GameObject victimObject = damageReport.victim.gameObject;
				CharacterBody victimBody = damageReport.victimBody;
				Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;

				if (attackerBody && attackerMaster)
				{
					if (victimBody)
					{
						if (Crowbar.enabled && Crowbar.crowbarManager)
                        {
							Crowbar.crowbarManager.Remove(victimBody.healthComponent);
                        }

						if (attackerInventory && OnCharacterDeathInventoryActions != null)
                        {
							OnCharacterDeathInventoryActions.Invoke(self, damageReport, attackerBody, attackerInventory, victimBody);
						}
						if (AssistManager.initialized && RiskyMod.assistManager)
						{
							//On-death is handled by assist manager to prevent having a bunch of duplicated code.
							//Need to add an assist here since it's called before OnHitEnemy.
							RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.assistLength);
							if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) > DamageType.Generic)
							{
								RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, BanditSpecialGracePeriod.duration, AssistManager.DirectAssistType.ResetCooldowns);
							}
							if ((damageInfo.damageType & DamageType.GiveSkullOnKill) > DamageType.Generic)
							{
								RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, BanditSpecialGracePeriod.duration, AssistManager.DirectAssistType.BanditSkull);
							}
							if (damageInfo.HasModdedDamageType(Bandit2Core.ResetRevolverOnKill))
							{
								RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, BanditSpecialGracePeriod.duration, AssistManager.DirectAssistType.ResetSpecial);
							}
							if (damageInfo.HasModdedDamageType(SharedDamageTypes.CrocoBiteHealOnKill))
							{
								RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, AssistManager.directAssistLength, AssistManager.DirectAssistType.CrocoBiteHealOnKill);
							}
							RiskyMod.assistManager.TriggerAssists(victimBody, attackerBody, damageInfo);
						}
					}
				}
			}
        }
	}
}
