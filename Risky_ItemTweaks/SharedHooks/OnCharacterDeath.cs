using Risky_ItemTweaks.Items.Boss;
using Risky_ItemTweaks.Items.Legendary;
using Risky_ItemTweaks.Items.Uncommon;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Risky_ItemTweaks.SharedHooks
{
    public class OnCharacterDeath
    {
        public static void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            CharacterBody attackerBody = damageReport.attackerBody;
            CharacterMaster attackerMaster = damageReport.attackerMaster;
			TeamIndex attackerTeamIndex = damageReport.attackerTeamIndex;
			DamageInfo damageInfo = damageReport.damageInfo;
			GameObject victimObject = damageReport.victim.gameObject;
			CharacterBody victimBody = damageReport.victimBody;
			Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;

			if (attackerBody && attackerMaster)
			{
				if (WillOWisp.enabled)
				{
					int wispCount = attackerInventory.GetItemCount(RoR2Content.Items.ExplodeOnDeath);
					if (wispCount > 0)
					{
						Vector3 victimPosition = Util.GetCorePosition(victimObject);
						float damageCoefficient = 3.5f * (1f + (float)(wispCount - 1) * 0.8f);
						float baseDamage = Util.OnKillProcDamage(attackerBody.damage, damageCoefficient);
						GameObject explosionPrefab = UnityEngine.Object.Instantiate<GameObject>(GlobalEventManager.instance.explodeOnDeathPrefab, victimPosition, Quaternion.identity);
						DelayBlast db = explosionPrefab.GetComponent<DelayBlast>();
						db.position = victimPosition;
						db.baseDamage = baseDamage;
						db.baseForce = 2000f;
						db.bonusForce = Vector3.up * 1000f;
						db.radius = 16f;
						db.attacker = damageInfo.attacker;
						db.inflictor = null;
						db.crit = Util.CheckRoll(attackerBody.crit, attackerMaster);
						db.maxTimer = 0.5f;
						db.damageColorIndex = DamageColorIndex.Item;
						db.falloffModel = BlastAttack.FalloffModel.SweetSpot;
						db.procCoefficient = Risky_ItemTweaks.disableProcChains ? 0f : 1f;
						explosionPrefab.GetComponent<TeamFilter>().teamIndex = attackerTeamIndex;
						NetworkServer.Spawn(explosionPrefab);
					}
				}
				if (Berzerker.enabled)
                {
					int berzerkCount = attackerInventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
					if (berzerkCount > 0)
					{
						//Need to apply buff this way to prevent the visual from disappearing.
						int newBuffStack = Mathf.Min(attackerBody.GetBuffCount(Berzerker.berzerkBuff) + 1, 2 + 3 * berzerkCount);
						int foundBuffs = 0;
						foreach (CharacterBody.TimedBuff tb in attackerBody.timedBuffs)
                        {
							if (tb.buffIndex == Berzerker.berzerkBuff.buffIndex)
                            {
								tb.timer = 5f + 0.33f * foundBuffs;
								foundBuffs++;
                            }
                        }
						for (int i = 0; i < newBuffStack - foundBuffs; i++)
                        {
							attackerBody.AddTimedBuff(Berzerker.berzerkBuff, 5f + 0.33f * foundBuffs);
							foundBuffs++;
						}
                    }
                }
				if (victimBody)
                {
					if (damageReport.victimIsElite)
					{
						if (Headhunter.enabled)
						{
							int hhCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
							if (hhCount > 0)
							{
								float duration = 10f * hhCount;
								for (int l = 0; l < BuffCatalog.eliteBuffIndices.Length; l++)
								{
									BuffIndex buffIndex = BuffCatalog.eliteBuffIndices[l];
									if (victimBody.HasBuff(buffIndex))
									{
										attackerBody.AddTimedBuff(buffIndex, duration);
										attackerBody.AddTimedBuff(Headhunter.headhunterBuff.buffIndex, duration);
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
