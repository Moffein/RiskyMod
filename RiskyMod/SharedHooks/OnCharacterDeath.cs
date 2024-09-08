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
				DamageInfo damageInfo = damageReport.damageInfo;
				CharacterBody victimBody = damageReport.victimBody;

				if (attackerBody && attackerMaster)
				{
					if (victimBody)
					{
						if (Crowbar.crowbarManager)
                        {
							Crowbar.crowbarManager.Remove(victimBody.healthComponent);
                        }

                        Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;
                        if (attackerInventory)
                        {
							OnCharacterDeathInventoryActions?.Invoke(self, damageReport, attackerBody, attackerInventory, victimBody);
						}
					}
				}
			}
        }
	}
}
