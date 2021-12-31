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
		public QueensGland()
		{
			if (!enabled) return;
			{
				if (!enabled) return;
				HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BeetleGland);

				On.RoR2.CharacterBody.UpdateBeetleGuardAllies += (orig, self) =>
				{
					if (NetworkServer.active)
					{
						SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
						if (sd && sd.baseSceneName.Equals("bazaar"))
						{
							return;
						}
						int glandCount = self.inventory ? self.inventory.GetItemCount(RoR2Content.Items.BeetleGland) : 0;
						if (glandCount > 0)
						{
							int deployableCount = self.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly);
							if (deployableCount < 1)    //used to be < glandCount
						{
								self.guardResummonCooldown -= Time.fixedDeltaTime;
								if (self.guardResummonCooldown <= 0f)
								{
									DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
									{
										placementMode = DirectorPlacementRule.PlacementMode.Approximate,
										minDistance = 3f,
										maxDistance = 40f,
										spawnOnTarget = self.transform
									}, RoR2Application.rng);
									directorSpawnRequest.summonerBodyObject = self.gameObject;
									directorSpawnRequest.ignoreTeamMemberLimit = true;  //Guards should always be able to spawn. Probably doesn't need a cap since there's only 1 per player.

								directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
									{
										Inventory guardInv = spawnResult.spawnedInstance.GetComponent<Inventory>();

										if (glandCount > 1)
										{
											guardInv.GiveItem(RoR2Content.Items.BoostDamage, 30 * glandCount);
											guardInv.GiveItem(RoR2Content.Items.BoostHp, 10 * glandCount);
											guardInv.GiveItem(RoR2Content.Items.UseAmbientLevel);
										}

										Deployable d = spawnResult.spawnedInstance.AddComponent<Deployable>();
										self.master.AddDeployable(d, DeployableSlot.BeetleGuardAlly);
									}));

									DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

									if (deployableCount < 1)
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
	}
}
