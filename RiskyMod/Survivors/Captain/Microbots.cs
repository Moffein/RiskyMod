using RoR2;
using RoR2.Projectile;
using UnityEngine;
using System.Collections.Generic;
using EntityStates.CaptainDefenseMatrixItem;
using EntityStates;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using RiskyMod.Items;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Captain
{
    public class Microbots
    {
        public static bool deletionRestrictions = true;
		public static bool droneScaling = true;

        public Microbots()
        {
            if (deletionRestrictions)
			{
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
						if (!projectileController.cannotBeDeleted && projectileController.teamFilter.teamIndex != teamIndex && (projectileController.transform.position - vector).sqrMagnitude < num)
						{
							bool canDelete = true;

							ProjectileSimple ps = projectileController.gameObject.GetComponent<ProjectileSimple>();
							ProjectileCharacterController pcc = projectileController.gameObject.GetComponent<ProjectileCharacterController>();

							if ((!ps || (ps && ps.desiredForwardSpeed == 0f)) && !pcc)
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

			if (droneScaling)
			{
				ItemsCore.ModifyItemDefActions += ModifyItem;

				var getMicrobotRechargeFrequency =
					new Hook(typeof(EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn).GetMethodCached("get_rechargeFrequency"), typeof(Microbots).GetMethodCached(nameof(MicrobotsAttackSpeedHook)));
			}
		}

		private static void ModifyItem()
		{
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.CaptainDefenseMatrix);
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.CaptainDefenseMatrix);
		}

		private static float MicrobotsAttackSpeedHook(EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn self)
        {
			float allyFactor = 1f;
			if (self.characterBody && self.characterBody.master)
			{
				ItemIndex droneMeldStackItem = ItemCatalog.FindItemIndex("DronemeldInternalStackItem");
				//based on https://github.com/DestroyedClone/RoR1SkillsPort/blob/master/Loader/ActivateShield.cs
				foreach (CharacterMaster characterMaster in CharacterMaster.readOnlyInstancesList)
				{
					if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == self.characterBody.master)
					{
						CharacterBody minionBody = characterMaster.GetBody();
						if (minionBody && !minionBody.isPlayerControlled && (minionBody.bodyFlags &= CharacterBody.BodyFlags.Mechanical) == CharacterBody.BodyFlags.Mechanical)
						{
							int mult = 1;
							if (minionBody.inventory && droneMeldStackItem != ItemIndex.None)
							{
								mult += minionBody.inventory.GetItemCount(droneMeldStackItem);
							}

							allyFactor += 0.2f * mult;
						}
					}
				}
			}
			return DefenseMatrixOn.baseRechargeFrequency / allyFactor;
		}
    }
}
