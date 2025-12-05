using RiskyMod;
using RiskyMod.Allies;
using RoR2;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.MegaDrone
{
	public class MegaDroneDeathState : GenericCharacterDeath
	{
		public int droneUpgradeCount = 0;
		public override void OnEnter()
		{
			if (NetworkServer.active)
			{
				if (characterBody && characterBody.inventory)
				{
					droneUpgradeCount = characterBody.inventory.GetItemCountPermanent(DLC3Content.Items.DroneUpgradeHidden);
				}
				//Based on ZetTweaks https://github.com/William758/ZetTweaks/blob/main/GameplayModule.cs
				if (base.transform)
				{
					DirectorPlacementRule placementRule = new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
						position = base.transform.position,
					};

					int spawnCount = 1;

					//Probably obsolete due to vanilla drone scrapper
					/*if (base.characterBody && base.characterBody.inventory)
                    {
                        ItemIndex droneMeldIndex = ItemCatalog.FindItemIndex("DronemeldInternalStackItem");
                        if (droneMeldIndex != ItemIndex.None)
                        {
                            spawnCount += base.characterBody.inventory.GetItemCount(droneMeldIndex);
                        }

                        ItemIndex minionMeldIndex = ItemCatalog.FindItemIndex("MinionMeldInternalStackItem");
                        if (minionMeldIndex != ItemIndex.None)
                        {
                            spawnCount += base.characterBody.inventory.GetItemCount(minionMeldIndex);
                        }
                    }*/

                    Vector3? desiredPosition = null;
					Quaternion? desiredRotation = null;
					for (int i = 0; i < spawnCount; i++)
					{
						GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, new Xoroshiro128Plus(0UL)));
						if (gameObject)
						{
							PurchaseInteraction purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
							if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money)
							{

								//Jank, share the cost coefficient of vanilla drones
								float costCoefficient = new EntityStates.Drone.DeathState().costCoefficient;


                                float upgradeScalar = 1f + (float)droneUpgradeCount * 0.2f;
                                purchaseInteraction.Networkcost = Mathf.RoundToInt((float)Run.instance.GetDifficultyScaledCost(purchaseInteraction.cost) * costCoefficient * upgradeScalar);
							}

							if (desiredPosition == null)
							{
								desiredPosition = gameObject.transform.position;
							}
							else
							{
								gameObject.transform.position = (Vector3)desiredPosition;
							}

							if (desiredRotation == null)
							{
								gameObject.transform.rotation = base.transform.rotation;
								desiredRotation = gameObject.transform.rotation;
							}
							else
							{
								gameObject.transform.rotation = (Quaternion)desiredRotation;
							}

							SummonMasterBehavior smb = gameObject.GetComponent<SummonMasterBehavior>();
							smb.droneUpgradeCount = droneUpgradeCount;
						}
					}
				}

				//Added DestroyOnTimer below.
				EntityState.Destroy(base.gameObject);
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			Util.PlaySound(MegaDroneDeathState.initialSoundString, base.gameObject);
			Transform modelTransform = base.GetModelTransform();

			if (modelTransform)
			{
				modelTransform.gameObject.AddComponent<DestroyOnTimer>().duration = 15f;
				if (NetworkServer.active)
				{
					ChildLocator component = modelTransform.GetComponent<ChildLocator>();
					if (component)
					{
						component.FindChild("LeftJet").gameObject.SetActive(false);
						component.FindChild("RightJet").gameObject.SetActive(false);
						//modelTransform.gameObject.SetActive(false);

						if (MegaDroneDeathState.initialEffect)
						{
							EffectManager.SpawnEffect(MegaDroneDeathState.initialEffect, new EffectData
							{
								origin = base.transform.position,
								scale = MegaDroneDeathState.initialEffectScale
							}, true);
						}
					}
				}
			}

			//Disable gibs
			Rigidbody component2 = base.GetComponent<Rigidbody>();
			RagdollController component3 = modelTransform.GetComponent<RagdollController>();

			if (component3 && component2)
			{
				component3.BeginRagdoll(component2.velocity * MegaDroneDeathState.velocityMagnitude);
			}
			ExplodeRigidbodiesOnStart component4 = modelTransform.GetComponent<ExplodeRigidbodiesOnStart>();
			if (component4)
			{
				component4.force = MegaDroneDeathState.explosionForce;
				component4.enabled = true;
			}
		}

		public static SpawnCard spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Drones/iscBrokenMegaDrone.asset").WaitForCompletion();
        public static string initialSoundString = "Play_drone_deathpt1";
		public static GameObject initialEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXDroneDeath.prefab").WaitForCompletion();
        public static float initialEffectScale = 15f;
		public static float velocityMagnitude = 1f;
		public static float explosionForce = 60000f;
	}
}
