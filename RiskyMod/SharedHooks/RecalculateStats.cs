using RiskyMod.Items.Uncommon;
using RiskyMod.Tweaks;
using RiskyMod.Tweaks.CharacterMechanics;
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

			if (TrueOSP.enabled && self.hasOneShotProtection)
			{
				if (self.cursePenalty > 1f || self.HasBuff(RoR2Content.Buffs.AffixLunar) ||	//This part doesn't need the inventory
					(self.inventory && self.inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0))	//This part needs the inventory
				{
					self.hasOneShotProtection = false;
				}
			}
		}
    }
}
