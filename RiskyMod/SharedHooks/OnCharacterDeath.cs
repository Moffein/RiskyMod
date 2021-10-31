using RiskyMod.Items.Boss;
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
				TeamIndex attackerTeamIndex = damageReport.attackerTeamIndex;
				DamageInfo damageInfo = damageReport.damageInfo;
				GameObject victimObject = damageReport.victim.gameObject;
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
							if (HeadHunter.enabled)
							{
								int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
								if (itemCount > 0)
								{
									RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.AssistType.HeadHunter, AssistManager.genericAssistLength);
								}
							}
							if (Brainstalks.enabled)
							{
								int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.KillEliteFrenzy);
								if (itemCount > 0)
								{
									RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.AssistType.Brainstalks, AssistManager.genericAssistLength);
								}
							}
							if (Berzerker.enabled)
							{
								int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
								if (itemCount > 0)
								{
									RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.AssistType.Berzerk, AssistManager.genericAssistLength);
								}
							}
							if (LaserTurbine.enabled)
							{
								int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.LaserTurbine);
								if (itemCount > 0)
								{
									RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.AssistType.LaserTurbine, AssistManager.genericAssistLength);
								}
							}
							if (FrostRelic.enabled)
							{
								int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.Icicle);
								if (itemCount > 0)
								{
									RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.AssistType.FrostRelic, AssistManager.genericAssistLength);
								}
							}
							if (BanditSpecialGracePeriod.enabled)
							{
								if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) > DamageType.Generic)
								{
									RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.AssistType.ResetCooldowns, BanditSpecialGracePeriod.duration);
								}
								if ((damageInfo.damageType & DamageType.GiveSkullOnKill) > DamageType.Generic)
								{
									RiskyMod.assistManager.AddAssist(attackerBody, victimBody, AssistManager.AssistType.BanditSkull, BanditSpecialGracePeriod.duration);
								}
							}

							RiskyMod.assistManager.TriggerAssists(victimBody, attackerBody, damageInfo);
						}
					}
				
					if (attackerInventory)
                    {
						if (HarvesterScythe.enabled)
                        {
							int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.HealOnCrit);
							if (itemCount > 0)
                            {
								attackerBody.AddTimedBuff(HarvesterScythe.scytheBuff, 1f + 1f * itemCount);
								EffectManager.SpawnEffect(HarvesterScythe.effectPrefab, new EffectData
								{
									origin = attackerBody.corePosition
								}, true);
							}
                        }
                    }
				}
			}
        }
    }
}
