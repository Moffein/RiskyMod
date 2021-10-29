using RiskyMod.Items.Boss;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Uncommon;
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

            CharacterBody attackerBody = damageReport.attackerBody;
            CharacterMaster attackerMaster = damageReport.attackerMaster;
			TeamIndex attackerTeamIndex = damageReport.attackerTeamIndex;
			DamageInfo damageInfo = damageReport.damageInfo;
			GameObject victimObject = damageReport.victim.gameObject;
			CharacterBody victimBody = damageReport.victimBody;
			Inventory attackerInventory = attackerMaster ? attackerMaster.inventory : null;

			if (attackerBody && attackerMaster)
			{
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
								tb.timer = 6f + foundBuffs;
								foundBuffs++;
                            }
                        }
						for (int i = 0; i < newBuffStack - foundBuffs; i++)
                        {
							attackerBody.AddTimedBuff(Berzerker.berzerkBuff, 6f + foundBuffs);
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
								float duration = 5f + 5f * hhCount;
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
