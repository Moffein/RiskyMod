using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using System;
using R2API;
using System.Runtime.CompilerServices;

namespace RiskyMod.Items.Boss
{
	public class QueensGland
	{
		public static bool enabled = true;
		public static bool ignoreAllyCap = true;
		public QueensGland()
		{
			if (!enabled) return;
			{
				if (!enabled) return;
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

				//Overwrite the Vanilla code since it's obfuscated in DNSPY
				On.RoR2.Items.BeetleGlandBodyBehavior.FixedUpdate += (orig, self) =>
				{
					if (NetworkServer.active)
					{
						SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
						if (sd && sd.baseSceneName.Equals("bazaar"))
						{
							return;
						}
						if (self.body.master)
                        {
							if (self.body.master.IsDeployableSlotAvailable(DeployableSlot.BeetleGuardAlly))    //used to be < glandCount
							{
								self.guardResummonCooldown -= Time.fixedDeltaTime;
								if (self.guardResummonCooldown <= 0f)
								{
									DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
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
										if (spawnResult.success && self.body.inventory)
										{
											Inventory guardInv = spawnResult.spawnedInstance.GetComponent<Inventory>();

											int glandCount = self.body.inventory ? self.body.inventory.GetItemCount(RoR2Content.Items.BeetleGland) : 0;
											if (guardInv && glandCount > 0)
											{

												int baseDamage = SoftDependencies.QueensGlandBuffLoaded ? GetInitialDamageCount() : 30;
												int stackDamage = SoftDependencies.QueensGlandBuffLoaded ? GetStackDamageCount() : 30;
												int baseHealth = SoftDependencies.QueensGlandBuffLoaded ? GetInitialHPCount() : 10;
												int stackHealth = SoftDependencies.QueensGlandBuffLoaded ? GetStackHPCount() : 10;

												int stackCount = glandCount - 1;

												guardInv.GiveItem(RoR2Content.Items.BoostDamage, baseDamage + stackCount * stackDamage);
												guardInv.GiveItem(RoR2Content.Items.BoostHp, baseHealth + stackCount * stackHealth);
												if (guardInv.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) guardInv.GiveItem(RoR2Content.Items.UseAmbientLevel);
											}

											Deployable d = spawnResult.spawnedInstance.AddComponent<Deployable>();
											self.body.master.AddDeployable(d, DeployableSlot.BeetleGuardAlly);

											CharacterMaster cm = spawnResult.spawnedInstance.GetComponent<CharacterMaster>();
											if (cm)
                                            {
												CharacterBody body = cm.GetBody();
												if (body)
												{
													UpdateGlandStats ugs = spawnResult.spawnedInstance.AddComponent<UpdateGlandStats>();
													ugs.ownerInventory = self.body.inventory;
													ugs.minionInventory = guardInv;
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
							}
						}
					}
				};
			}
		}

		private static void ModifyItem()
        {
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BeetleGland);
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static int GetInitialDamageCount()
		{
			return QueenGlandBuff.MainPlugin.Config_BaseDamage.Value;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static int GetStackDamageCount()
		{
			return QueenGlandBuff.MainPlugin.Config_StackDamage.Value;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static int GetInitialHPCount()
		{
			return QueenGlandBuff.MainPlugin.Config_BaseHealth.Value;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static int GetStackHPCount()
		{
			return QueenGlandBuff.MainPlugin.Config_StackHealth.Value;
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
				int targetHealthBoost = 10 * glandCount;
				int targetDamageBoost = 30 * glandCount;

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
