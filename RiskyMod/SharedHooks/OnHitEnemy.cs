using RiskyMod.Items.Uncommon;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using RiskyMod.Fixes;
using R2API;
using RiskyMod.Items.Common;
using RiskyMod.Items.Legendary;
using RiskyMod.MonoBehaviours;
using RiskyMod.Tweaks;

namespace RiskyMod.SharedHooks
{
    class OnHitEnemy
    {
        public static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
			CharacterBody attackerBody = null;
			CharacterBody victimBody = null;
			Inventory attackerInventory = null;

			bool validDamage = NetworkServer.active && damageInfo.procCoefficient > 0f && !damageInfo.rejected;

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
                    }
				}
			}

			orig(self, damageInfo, victim);

            if (validDamage)
            {
                if (damageInfo.attacker)
                {
                    if (attackerBody && victimBody)
					{
						//Vector3 aimOrigin = attackerBody.aimOrigin;
						//CharacterMaster attackerMaster = attackerBody.master;
						TeamComponent attackerTeamComponent = attackerBody.teamComponent;
						//TeamIndex attackerTeamIndex = attackerTeamComponent ? attackerTeamComponent.teamIndex : TeamIndex.None;

						if (attackerInventory)
                        {
							if (AssistManager.initialized && RiskyMod.assistManager)
							{
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
							}
						}
                    }
                }
            }
        }
    }
}
