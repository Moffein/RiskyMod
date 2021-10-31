using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Uncommon;
using RiskyMod.MonoBehaviours;
using RiskyMod.Tweaks;
using RoR2;
using RoR2.Projectile;
using System;
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
						if (attackerInventory)
                        {
							if (Incubator.enabled)
                            {
								int incubatorOnKillCount = attackerMaster.inventory.GetItemCount(RoR2Content.Items.Incubator);
								if (incubatorOnKillCount > 0 && attackerMaster.GetDeployableCount(DeployableSlot.ParentPodAlly) + attackerMaster.GetDeployableCount(DeployableSlot.ParentAlly) < incubatorOnKillCount && Util.CheckRoll(7f + 1f * (float)incubatorOnKillCount, attackerMaster))
								{
									DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscParentPod"), new DirectorPlacementRule
									{
										placementMode = DirectorPlacementRule.PlacementMode.Approximate,
										minDistance = 3f,
										maxDistance = 20f,
										spawnOnTarget = victimBody.gameObject.transform
									}, RoR2Application.rng);
									directorSpawnRequest.summonerBodyObject = attackerBody.gameObject;
									directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
									{
										Inventory inventory = spawnResult.spawnedInstance.GetComponent<CharacterMaster>().inventory;
										inventory.GiveItem(RoR2Content.Items.BoostDamage, 30);
										inventory.GiveItem(RoR2Content.Items.BoostHp, 10 * incubatorOnKillCount);
										inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
									}));
									directorSpawnRequest.ignoreTeamMemberLimit = true;
									DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
								}
							}
                        }
						if (AssistManager.initialized && RiskyMod.assistManager)
						{
							//On-death is handled by assist manager to prevent having a bunch of duplicated code.
							//Need to add an assist here since it's called before OnHitEnemy.
							RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.assistLength);
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
