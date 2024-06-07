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
		public static OnHitNoAttacker OnHitNoAttackerActions;

		public delegate void OnHitAttacker(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody);
		public static OnHitAttacker OnHitAttackerActions;

		public delegate void OnHitAttackerInventory(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory);
		public static OnHitAttackerInventory OnHitAttackerInventoryActions;

		public static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
			CharacterBody attackerBody = null;
			CharacterBody victimBody = null;
			Inventory attackerInventory = null;

			bool validDamage = NetworkServer.active && damageInfo.procCoefficient > 0f && !damageInfo.rejected;
			bool assistsEnabled = RiskyMod.assistManager != null;

			if (validDamage)
			{
				if (damageInfo.attacker)
				{
					attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
					victimBody = victim ? victim.GetComponent<CharacterBody>() : null;

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

					//This needs a refactor so that it gets hooked by the things that specifically add it, instead of having the things here.
					//Run this before triggering on-hit procs so that procs don't kill enemies before this triggers.
					if (assistsEnabled)
                    {
						if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) > DamageType.Generic)
						{
							RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, BanditSpecialGracePeriod.GetDuration(damageInfo.attacker), AssistManager.DirectAssistType.ResetCooldowns);
						}
						if ((damageInfo.damageType & DamageType.GiveSkullOnKill) > DamageType.Generic)
						{
							RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, BanditSpecialGracePeriod.GetDuration(damageInfo.attacker), AssistManager.DirectAssistType.BanditSkull);
						}
						if (damageInfo.HasModdedDamageType(SharedDamageTypes.CrocoBiteHealOnKill))
                        {
							RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, AssistManager.directAssistLength, AssistManager.DirectAssistType.CrocoBiteHealOnKill);
						}
						if (damageInfo.HasModdedDamageType(Bandit2Core.StandoffDamage))
						{
                            RiskyMod.assistManager.AddDirectAssist(attackerBody, victimBody, BanditSpecialGracePeriod.GetDuration(damageInfo.attacker), AssistManager.DirectAssistType.BanditStandoff);
                        }
					}
				}
			}

			orig(self, damageInfo, victim);

            if (validDamage && victimBody)
            {
                OnHitNoAttackerActions?.Invoke(damageInfo, victimBody);

                if (damageInfo.attacker && attackerBody)
                {
                    if (OnHitAttackerActions != null) OnHitAttackerActions.Invoke(damageInfo, victimBody, attackerBody);
                    if (attackerInventory && OnHitAttackerInventoryActions != null) OnHitAttackerInventoryActions.Invoke(damageInfo, victimBody, attackerBody, attackerInventory);
                }
            }
        }
    }
}
