using EntityStates.Drone.DroneWeapon;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DroneChanges
{
    public class MegaDrone
    {
		public static bool allowRepair = true;
		public static GameObject explosionEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick");
		public static DamageAPI.ModdedDamageType MegaTurretExplosion;

		public MegaDrone()
		{
			GameObject megaDroneMasterObject = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/MegaDroneMaster");

			AISkillDriver[] aiDrivers = megaDroneMasterObject.GetComponentsInChildren<AISkillDriver>();
            for (int i = 0; i < aiDrivers.Length; i++)
            {
                if (aiDrivers[i].customName == "StopTooCloseTarget")
                {
                    aiDrivers[i].movementType = AISkillDriver.MovementType.StrafeMovetarget;
					aiDrivers[i].noRepeat = true;
					aiDrivers[i].resetCurrentEnemyOnNextDriverSelection = true; //Does this actually improve it?
                    break;
                }
            }

			GameObject megaDroneBrokenObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/MegaDroneBroken.prefab").WaitForCompletion();
			PurchaseInteraction pi = megaDroneBrokenObject.GetComponent<PurchaseInteraction>();
			pi.cost = 300;  //Vanilla is 350

			GameObject megaDroneBodyObject = AllyPrefabs.MegaDrone;
			if (allowRepair)
            {
				CharacterDeathBehavior cdb = megaDroneBodyObject.GetComponent<CharacterDeathBehavior>();
				//cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Drone.DeathState));
				cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.MegaDrone.MegaDroneDeathState));
				Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.MegaDrone.MegaDroneDeathState));
			}

			CharacterBody megaDroneBody = megaDroneBodyObject.GetComponent<CharacterBody>();
			megaDroneBody.baseArmor = 20f;
			megaDroneBody.baseRegen = megaDroneBody.baseMaxHealth / 30f;
			megaDroneBody.levelRegen = megaDroneBody.baseRegen * 0.2f;
			megaDroneBody.baseMaxShield = megaDroneBody.baseMaxHealth * 0.08f;
			megaDroneBody.levelMaxShield = megaDroneBody.baseMaxShield * 0.3f;

			UpgradeMegaTurret();
		}

		private void UpgradeMegaTurret()
        {
			MegaTurretExplosion = DamageAPI.ReserveDamageType();
			SharedHooks.OnHitAll.HandleOnHitAllActions += MegaTurretExplosionOnHitAll;

			On.EntityStates.Drone.DroneWeapon.FireMegaTurret.OnEnter += (orig, self) =>
			{
				orig(self);
				self.totalDuration = FireMegaTurret.baseTotalDuration;
			};

			IL.EntityStates.Drone.DroneWeapon.FireMegaTurret.FireBullet += (il) =>
			{
				ILCursor c = new ILCursor(il);
				if (c.TryGotoNext(
					 x => x.MatchCallvirt<BulletAttack>("Fire")
					 ))
				{
					c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
					{
						bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
						bulletAttack.damage *= 0.25f;
						bulletAttack.radius = 0.5f;
						bulletAttack.smartCollision = true;
						bulletAttack.AddModdedDamageType(MegaDrone.MegaTurretExplosion);
						return bulletAttack;
					});
				}
				else
				{
					Debug.LogError("RiskyMod: ModifyBulletAttacks DroneWeapon.FireMegaTurret IL Hook failed");
				}
			};
		}

		private static void MegaTurretExplosionOnHitAll(GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
			if (damageInfo.HasModdedDamageType(MegaDrone.MegaTurretExplosion))
            {
				EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
				{
					origin = damageInfo.position,
					scale = 3f,
					rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
				}, true);
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.position = damageInfo.position;
				blastAttack.baseDamage = damageInfo.damage * 3f;
				blastAttack.baseForce = 0f;
				blastAttack.radius = 3f;
				blastAttack.attacker = damageInfo.attacker;
				blastAttack.inflictor = null;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.crit = damageInfo.crit;
				blastAttack.procChainMask = damageInfo.procChainMask;
				blastAttack.procCoefficient = 0f;
				blastAttack.damageColorIndex = DamageColorIndex.Default;
				blastAttack.falloffModel = BlastAttack.FalloffModel.None;
				blastAttack.damageType = damageInfo.damageType;
				blastAttack.Fire();
			}
        }
	}
}

namespace EntityStates.RiskyMod.MegaDrone
{
	public class MegaDroneDeathState : GenericCharacterDeath
	{
		public override void OnEnter()
		{
			if (NetworkServer.active)
			{
				//Based on ZetTweaks https://github.com/William758/ZetTweaks/blob/main/GameplayModule.cs
				if (base.transform)
				{
					DirectorPlacementRule placementRule = new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
						position = base.transform.position,
					};

					GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, new Xoroshiro128Plus(0UL)));
					if (gameObject)
					{
						PurchaseInteraction purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
						if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money)
						{
							purchaseInteraction.Networkcost = Run.instance.GetDifficultyScaledCost(purchaseInteraction.cost);
						}

						gameObject.transform.rotation = base.transform.rotation;
					}
				}

				EntityState.Destroy(base.gameObject);
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			Util.PlaySound(MegaDroneDeathState.initialSoundString, base.gameObject);
			Transform modelTransform = base.GetModelTransform();

			base.gameObject.AddComponent<DestroyOnTimer>().duration = 15f;
			if (modelTransform)
			{
				modelTransform.gameObject.AddComponent<DestroyOnTimer>().duration = 15f;
				if (NetworkServer.active)
				{
					ChildLocator component = modelTransform.GetComponent<ChildLocator>();
					if (component)
					{
						//component.FindChild("LeftJet").gameObject.SetActive(false);
						//component.FindChild("RightJet").gameObject.SetActive(false);
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

		public static SpawnCard spawnCard = LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscBrokenMegaDrone");
		public static string initialSoundString = "Play_drone_deathpt1";
		public static GameObject initialEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXDroneDeath.prefab").WaitForCompletion();
		public static float initialEffectScale = 15f;
		public static float velocityMagnitude = 1f;
		public static float explosionForce = 60000f;
	}
}
