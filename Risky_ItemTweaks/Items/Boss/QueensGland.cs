using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using System;
using R2API;

namespace Risky_Mod.Items.Boss
{
    public class QueensGland
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

			LanguageAPI.Add("ITEM_BEETLEGLAND_DESC", "<style=cIsUtility>Recruit a Beetle Guard</style> with bonus <style=cIsDamage>300%</style> <style=cStack>(+300% per stack)</style> damage and <style=cIsHealing>100% <style=cStack>(+100% per stack)</style> health</style>.");

			On.RoR2.CharacterBody.UpdateBeetleGuardAllies += (orig, self) =>
            {
				if (NetworkServer.active)
				{
					int glandCount = self.inventory ? self.inventory.GetItemCount(RoR2Content.Items.BeetleGland) : 0;
					if (glandCount > 0)
					{
						int deployableCount = self.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly);
						if (deployableCount < 1)	//used to be < glandCount
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

								DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;

								directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
								{
									Inventory guardInv = spawnResult.spawnedInstance.GetComponent<Inventory>();

									if (glandCount > 1)
									{
										guardInv.GiveItem(RoR2Content.Items.BoostDamage, 30 * glandCount);
										guardInv.GiveItem(RoR2Content.Items.BoostHp, 10 * glandCount);
									}

									Deployable d = spawnResult.spawnedInstance.AddComponent<Deployable>();
									self.master.AddDeployable(d, DeployableSlot.BeetleGuardAlly);
								}));

								DirectorCore.instance.TrySpawnObject(directorSpawnRequest2);

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
