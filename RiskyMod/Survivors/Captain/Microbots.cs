using RoR2;
using RoR2.Projectile;
using UnityEngine;
using System.Collections.Generic;
using EntityStates.CaptainDefenseMatrixItem;
using EntityStates;

namespace RiskyMod.Survivors.Captain
{
    public class Microbots
    {
        public static bool enabled = true;
        public Microbots()
        {
            if (!enabled) return;
			On.EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.DeleteNearbyProjectile += (orig, self) =>
			{
				Vector3 vector = self.attachedBody ? self.attachedBody.corePosition : Vector3.zero;
				TeamIndex teamIndex = self.attachedBody ? self.attachedBody.teamComponent.teamIndex : TeamIndex.None;
				float num = DefenseMatrixOn.projectileEraserRadius * DefenseMatrixOn.projectileEraserRadius;
				int num2 = 0;
				int itemStack = self.GetItemStack();
				bool result = false;
				List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
				List<ProjectileController> list = new List<ProjectileController>();
				int num3 = 0;
				int count = instancesList.Count;
				while (num3 < count && num2 < itemStack)
				{
					ProjectileController projectileController = instancesList[num3];
					if (projectileController.teamFilter.teamIndex != teamIndex && (projectileController.transform.position - vector).sqrMagnitude < num)
					{
						bool canDelete = true;
						if (canDelete && !projectileController.gameObject.GetComponent<ProjectileSimple>())
						{
							canDelete = false;
						}

						if (canDelete)
                        {
							list.Add(projectileController);
							num2++;
						}
					}
					num3++;
				}
				int i = 0;
				int count2 = list.Count;
				while (i < count2)
				{
					ProjectileController projectileController2 = list[i];
					if (projectileController2)
					{
						result = true;
						Vector3 position = projectileController2.transform.position;
						Vector3 start = vector;
						if (DefenseMatrixOn.tracerEffectPrefab)
						{
							EffectData effectData = new EffectData
							{
								origin = position,
								start = start
							};
							EffectManager.SpawnEffect(DefenseMatrixOn.tracerEffectPrefab, effectData, true);
						}
						EntityState.Destroy(projectileController2.gameObject);
					}
					i++;
				}
				return result;
			};
        }
    }
}
