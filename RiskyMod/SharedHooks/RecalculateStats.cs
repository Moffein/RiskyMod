using RiskyMod.Items.Uncommon;
using RiskyMod.MonoBehaviours;
using RiskyMod.Tweaks;
using RoR2;
using UnityEngine;

namespace RiskyMod.SharedHooks
{
    public class RecalculateStats
    {
        public static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
			if (TrueOSP.enabled && self.hasOneShotProtection)
			{
				//Disable vanilla OSP
				self.oneShotProtectionFraction = 0f;
				if (self.HasBuff(TrueOSP.disableOSP))
				{
					if (self.outOfDanger)// && (self.healthComponent && self.healthComponent.health/self.healthComponent.fullHealth > OSPManagerComponent.ospThreshold)
					{
						//Currently not locked behind having >90% HP because I'd like it to be cleanly tied to the outOfDanger state.
						//Will have to see if this ends up being too abusable.
						self.RemoveBuff(TrueOSP.disableOSP);
					}
					else
					{
						self.hasOneShotProtection = false;
					}
				}
			}
			if (self.inventory)
            {
				Inventory inventory = self.inventory;
				if (Bandolier.enabled && inventory.GetItemCount(RoR2Content.Items.Bandolier) > 0)
				{
					if (self.skillLocator)
					{
						if (self.skillLocator.primary)
						{
							self.skillLocator.primary.cooldownScale *= 0.85f;
						}
						if (self.skillLocator.secondary)
						{
							self.skillLocator.secondary.cooldownScale *= 0.85f;
						}
						if (self.skillLocator.utility)
						{
							self.skillLocator.utility.cooldownScale *= 0.85f;
						}
						if (self.skillLocator.special)
						{
							self.skillLocator.special.cooldownScale *= 0.85f;
						}
					}
				}

				//This happens after GetStatCoefficients; needed for the armor mult since that's not in GSC currently
				if (RoseBuckler.enabled && self.isSprinting)
                {
					if (inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0)
					{
						self.armor *= 1.5f;
					}
                }

				//This part is split off because it needs the inventory to work
				if (TrueOSP.enabled && self.hasOneShotProtection)
                {
					if (inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0)
                    {
						self.hasOneShotProtection = false;
                    }
                }
			}
		}
    }
}
