using RiskyMod.Items.Uncommon;
using RiskyMod.Tweaks;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class RecalculateStats
    {
        public static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			orig(self);
			if (self.inventory)
            {
				Inventory inventory = self.inventory;

				//This happens after GetStatCoefficients; needed for the armor mult since that's not in GSC currently
				if (RoseBuckler.enabled)
                {
					if (self.isSprinting && inventory.GetItemCount(RoR2Content.Items.SprintArmor) > 0)
					{
						self.armor *= 1.5f;
					}
                }
			}

			if (TrueOSP.enabled && self.hasOneShotProtection)
			{
				if (self.isGlass || self.cursePenalty > 1f || self.HasBuff(RoR2Content.Buffs.AffixLunar) ||	//This part doesn't need the inventory
					(self.inventory && self.inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0))	//This part needs the inventory
				{
					self.hasOneShotProtection = false;
				}
			}
		}
    }
}
