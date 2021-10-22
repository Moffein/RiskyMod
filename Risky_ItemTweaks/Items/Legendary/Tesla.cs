using UnityEngine;
using RoR2;
using RoR2.Orbs;
using UnityEngine.Networking;

namespace Risky_ItemTweaks.Items.Legendary
{
    public class Tesla
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            On.RoR2.CharacterBody.UpdateTeslaCoil += (orig, self) =>
            {
				if (!NetworkServer.active)
				{
					Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateTeslaCoil()' called on client");
					return;
				}
				if (self.inventory)
				{
					int itemCount = self.inventory.GetItemCount(RoR2Content.Items.ShockNearby);
					if (itemCount > 0)
					{
						self.teslaBuffRollTimer += Time.fixedDeltaTime;
						if (self.teslaBuffRollTimer >= 10f)
						{
							self.teslaBuffRollTimer = 0f;
							self.teslaCrit = Util.CheckRoll(self.crit, self.master);
							self.teslaIsOn = self.HasBuff(RoR2Content.Buffs.TeslaField);
							if (!self.teslaIsOn)
							{
								self.AddBuff(RoR2Content.Buffs.TeslaField);
							}
							else
							{
								self.RemoveBuff(RoR2Content.Buffs.TeslaField);
							}
						}
						if (self.HasBuff(RoR2Content.Buffs.TeslaField))
						{
							self.teslaFireTimer += Time.fixedDeltaTime;
							self.teslaResetListTimer += Time.fixedDeltaTime;
							if (self.teslaFireTimer >= 0.0833333358f)
							{
								self.teslaFireTimer = 0f;
								LightningOrb lightningOrb = new LightningOrb
								{
									origin = self.corePosition,
									damageValue = self.damage * 2f,
									isCrit = self.teslaCrit,
									bouncesRemaining = 2 * itemCount,
									teamIndex = self.teamComponent.teamIndex,
									attacker = self.gameObject,
									procCoefficient = Risky_ItemTweaks.disableProcChains ? 0f : 0.3f,
									bouncedObjects = self.previousTeslaTargetList,
									lightningType = LightningOrb.LightningType.Tesla,
									damageColorIndex = DamageColorIndex.Item,
									range = 20f
								};
								HurtBox hurtBox = lightningOrb.PickNextTarget(self.transform.position);
								if (hurtBox)
								{
									self.previousTeslaTargetList.Add(hurtBox.healthComponent);
									lightningOrb.target = hurtBox;
									OrbManager.instance.AddOrb(lightningOrb);
								}
							}
							if (self.teslaResetListTimer >= self.teslaResetListInterval)
							{
								self.teslaResetListTimer -= self.teslaResetListInterval;
								self.previousTeslaTargetList.Clear();
							}
						}
					}
				}
			};

			On.RoR2.ItemCatalog.Init += (orig) =>
			{
				orig();
				Risky_ItemTweaks.AddToAIBlacklist(RoR2Content.Items.ShockNearby.itemIndex);
			};
		}
    }
}
