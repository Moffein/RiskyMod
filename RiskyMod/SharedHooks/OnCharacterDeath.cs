using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Uncommon;
using RiskyMod.MonoBehaviours;
using RiskyMod.Tweaks;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class OnCharacterDeath
    {
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
						if (AssistManager.initialized && RiskyMod.assistManager)
						{
							//On-death is handled by assist manager to prevent having a bunch of duplicated code.
							//Need to add an assist here since it's called before OnHitEnemy.
							RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.genericAssistLength);
							if (BanditSpecialGracePeriod.enabled)
							{
								if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) > DamageType.Generic)
								{
									RiskyMod.assistManager.AddBanditAssist(attackerBody, victimBody, BanditSpecialGracePeriod.duration, AssistManager.BanditAssistType.ResetCooldowns);
								}
								if ((damageInfo.damageType & DamageType.GiveSkullOnKill) > DamageType.Generic)
								{
									RiskyMod.assistManager.AddBanditAssist(attackerBody, victimBody, BanditSpecialGracePeriod.duration, AssistManager.BanditAssistType.BanditSkull);
								}
							}

							RiskyMod.assistManager.TriggerAssists(victimBody, attackerBody, damageInfo);
						}
					}
				}
			}
        }
    }
}
