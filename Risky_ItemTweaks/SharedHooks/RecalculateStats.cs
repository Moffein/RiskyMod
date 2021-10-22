using Risky_ItemTweaks.Items.Uncommon;
using Risky_ItemTweaks.Tweaks;
using RoR2;
using UnityEngine;

namespace Risky_ItemTweaks.SharedHooks
{
    public class RecalculateStats
    {
        public static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
			if (self.inventory)
            {
				if (Bandolier.enabled && self.inventory.GetItemCount(RoR2Content.Items.Bandolier) > 0)
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

				//This happens after GSC; needed for the armor mult since that's not in GSC currently
				if (RoseBuckler.enabled && self.isSprinting)
                {
					self.armor *= 1.5f;
                }

				if (ShieldGating.enabled)
                {
					self.hasOneShotProtection = false;
					self.oneShotProtectionFraction = Mathf.Infinity;
                }
			}
		}
    }
}
