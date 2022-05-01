using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using System;
using R2API;

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
						return (self.inventory.GetItemCount(RoR2Content.Items.BeetleGland) > 0) ? 1 : 0;
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
											if (guardInv && glandCount > 1)
											{
												guardInv.GiveItem(RoR2Content.Items.BoostDamage, 30 * glandCount);
												guardInv.GiveItem(RoR2Content.Items.BoostHp, 10 * glandCount);
												if (guardInv.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) guardInv.GiveItem(RoR2Content.Items.UseAmbientLevel);
											}

											Deployable d = spawnResult.spawnedInstance.AddComponent<Deployable>();
											self.body.master.AddDeployable(d, DeployableSlot.BeetleGuardAlly);
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
	}
}
