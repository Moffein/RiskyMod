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

		public static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
			CharacterBody attackerBody = null;
			CharacterBody victimBody = null;
			Inventory attackerInventory = null;

			bool validDamage = NetworkServer.active && damageInfo.procCoefficient > 0f && !damageInfo.rejected;
            victimBody = victim.GetComponent<CharacterBody>();

			orig(self, damageInfo, victim);

            if (validDamage)
            {
                victimBody = victim.GetComponent<CharacterBody>();
                OnHitNoAttackerActions?.Invoke(damageInfo, victimBody);

                if (damageInfo.attacker)
                {
                    attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        OnHitAttackerActions?.Invoke(damageInfo, victimBody, attackerBody);

                        attackerInventory = attackerBody.inventory;
                        if (attackerInventory) OnHitAttackerInventoryActions?.Invoke(damageInfo, victimBody, attackerBody, attackerInventory);
                    }
                }
            }
        }
    }
}
