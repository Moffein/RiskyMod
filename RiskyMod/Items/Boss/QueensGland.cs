using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using System;
using R2API;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Boss
{
	public class QueensGland
	{
		public static bool enabled = true;
		public static bool ignoreAllyCap = true;
		public static BodyIndex BeetleGuardAllyIndex;

		public QueensGland()
		{
			if (!enabled || SoftDependencies.QueensGlandBuffLoaded)
			{
				HandleBeetleAllyVanilla();
				return;
			}
			ItemsCore.ModifyItemDefActions += ModifyItem;

			On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
			{
				if (slot == DeployableSlot.BeetleGuardAlly)
				{
					return ((self.inventory.GetItemCount(RoR2Content.Items.BeetleGland) > 0) ? 1 : 0) * (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef) ? 2 : 1);
				}
				else
				{
					return orig(self, slot);
				}
			};

			//Overwrite the Vanilla code since it's obfuscated in DNSpy
			On.RoR2.Items.BeetleGlandBodyBehavior.FixedUpdate += (orig, self) =>
			{
				if (!NetworkServer.active || RiskyMod.inBazaar || !(self.body.master && self.body.master.IsDeployableSlotAvailable(DeployableSlot.BeetleGuardAlly))) return;
				self.guardResummonCooldown -= Time.fixedDeltaTime;
				if (self.guardResummonCooldown <= 0f)
				{
					DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/BeetleGland/cscBeetleGuardAlly.asset").WaitForCompletion(), new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.Approximate,
						minDistance = 3f,
						maxDistance = 40f,
						spawnOnTarget = self.transform
					}, RoR2Application.rng);
                    directorSpawnRequest.summonerBodyObject = self.gameObject;
					directorSpawnRequest.ignoreTeamMemberLimit = ignoreAllyCap;  //Guards should always be able to spawn. Probably doesn't need a cap since there's only 1 per player.

					directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
                    {
                        if (spawnResult.success && spawnResult.spawnedInstance && self.body.inventory)
                        {
                            Inventory guardInv = spawnResult.spawnedInstance.GetComponent<Inventory>();
                            CharacterMaster master = spawnResult.spawnedInstance.GetComponent<CharacterMaster>();

                            int glandCount = self.body.inventory ? self.body.inventory.GetItemCount(RoR2Content.Items.BeetleGland) : 0;
							if (guardInv && glandCount > 0)
                            {
                                int baseDamage = 20;
								int stackDamage = 30;
								int baseHealth = 10;
								int stackHealth = 10;

								int stackCount = glandCount - 1;

								guardInv.GiveItem(RoR2Content.Items.BoostDamage, baseDamage + stackCount * stackDamage);
								guardInv.GiveItem(RoR2Content.Items.BoostHp, baseHealth + stackCount * stackHealth);
								if (guardInv.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) guardInv.GiveItem(RoR2Content.Items.UseAmbientLevel);
							}

                            Deployable d = spawnResult.spawnedInstance.AddComponent<Deployable>();
                            if (d && self.body.master.IsDeployableSlotAvailable(DeployableSlot.BeetleGuardAlly))
                            {
                                self.body.master.AddDeployable(d, DeployableSlot.BeetleGuardAlly);
                            }
							else
							{
								if (master) master.TrueKill();
								return;
							}

							if (master)
							{
								CharacterBody body = master.GetBody();
								if (body)
								{
									UpdateGlandStats ugs = spawnResult.spawnedInstance.AddComponent<UpdateGlandStats>();
									ugs.ownerInventory = self.body.inventory;
									ugs.minionInventory = guardInv;
								}

								if (guardInv && master.teamIndex == TeamIndex.Player)
								{
									guardInv.GiveItem(Allies.AllyItems.AllyMarkerItem);
									guardInv.GiveItem(Allies.AllyItems.AllyScalingItem);
									guardInv.GiveItem(Allies.AllyItems.AllyRegenItem, 40);
									guardInv.GiveItem(Allies.AllyItems.AllyAllowVoidDeathItem);
									guardInv.GiveItem(Allies.AllyItems.AllyAllowOverheatDeathItem);
								}
							}
						}
					}));

					DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

					if (self.body.master.IsDeployableSlotAvailable(DeployableSlot.BeetleGuardAlly))
					{
						self.guardResummonCooldown = 1f;
						return;
					}
					self.guardResummonCooldown = 30f;
				}
			};
		}

		private static void ModifyItem()
        {
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BeetleGland);
		}

		private void HandleBeetleAllyVanilla()
		{
			On.RoR2.CharacterBody.Start += (orig, self) =>
			{
				orig(self);
				if (NetworkServer.active && !self.isPlayerControlled && self.bodyIndex == QueensGland.BeetleGuardAllyIndex && self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player)
				{
					if (self.inventory)
					{
						self.inventory.GiveItem(Allies.AllyItems.AllyMarkerItem);
						self.inventory.GiveItem(Allies.AllyItems.AllyScalingItem);
						self.inventory.GiveItem(Allies.AllyItems.AllyRegenItem, 40);
						self.inventory.GiveItem(Allies.AllyItems.AllyAllowVoidDeathItem);
						self.inventory.GiveItem(Allies.AllyItems.AllyAllowOverheatDeathItem);
					}
				}
			};

			RoR2Application.onLoadFinished += OnLoad;
		}

		private void OnLoad()
		{
            QueensGland.BeetleGuardAllyIndex = BodyCatalog.FindBodyIndex("BeetleGuardAllyBody");
        }
	}

	public class UpdateGlandStats : MonoBehaviour
    {
		public Inventory minionInventory;
		public Inventory ownerInventory;

		public void FixedUpdate()
        {
			if (NetworkServer.active && ownerInventory && minionInventory)
            {
				int glandCount = Math.Max(ownerInventory.GetItemCount(RoR2Content.Items.BeetleGland), 1);
				int stackCount = glandCount - 1;

				int baseDamage = 20;
				int stackDamage = 30;
				int baseHealth = 10;
				int stackHealth = 10;

				int targetHealthBoost = baseDamage + stackCount * stackDamage;
				int targetDamageBoost = baseHealth + stackCount * stackHealth;

				int currentHealthBoost = minionInventory.GetItemCount(RoR2Content.Items.BoostHp);
				int currentDamageBoost = minionInventory.GetItemCount(RoR2Content.Items.BoostDamage);

				if (currentHealthBoost != targetHealthBoost)
				{
					if (currentHealthBoost < targetHealthBoost)
					{
						minionInventory.GiveItem(RoR2Content.Items.BoostHp, targetHealthBoost - currentHealthBoost);
					}
					else if (currentHealthBoost > targetHealthBoost)
					{
						minionInventory.RemoveItem(RoR2Content.Items.BoostHp, currentHealthBoost - targetHealthBoost);
					}
				}

				if (currentDamageBoost != targetDamageBoost)
				{
					if (currentDamageBoost < targetDamageBoost)
					{
						minionInventory.GiveItem(RoR2Content.Items.BoostDamage, targetDamageBoost - currentDamageBoost);
					}
					else if (currentDamageBoost > targetDamageBoost)
					{
						minionInventory.RemoveItem(RoR2Content.Items.BoostDamage, currentDamageBoost - targetDamageBoost);
					}
				}
			}
        }
    }
}
