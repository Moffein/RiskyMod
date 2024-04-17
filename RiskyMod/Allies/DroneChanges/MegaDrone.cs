using EntityStates.Drone.DroneWeapon;
using RoR2.Projectile;
using MonoMod.Cil;
using R2API;
using RiskyMod.Allies.DroneBehaviors;
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
				cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyModStates.MegaDrone.MegaDroneDeathState));
			}
			Content.Content.entityStates.Add(typeof(EntityStates.RiskyModStates.MegaDrone.MegaDroneDeathState));

			CharacterBody megaDroneBody = megaDroneBodyObject.GetComponent<CharacterBody>();
			megaDroneBody.baseArmor = 20f;
			megaDroneBody.baseRegen = megaDroneBody.baseMaxHealth / 30f;
			megaDroneBody.levelRegen = megaDroneBody.baseRegen * 0.2f;
			megaDroneBody.baseMaxShield = megaDroneBody.baseMaxHealth * 0.1f;
			megaDroneBody.levelMaxShield = megaDroneBody.baseMaxShield * 0.3f;

			UpgradeMegaTurret();
			ModifyRockets();
			AddPanicShield();
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
						bulletAttack.maxDistance = 1000f;
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

		private void ModifyRockets()
        {
			GameObject rocketPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocket.prefab").WaitForCompletion().InstantiateClone("RiskyModMegaDroneRocketProjectile", true);
			Content.Content.projectilePrefabs.Add(rocketPrefab);

			ProjectileImpactExplosion pie = rocketPrefab.GetComponent<ProjectileImpactExplosion>();
			pie.falloffModel = BlastAttack.FalloffModel.None;
			//pie.blastRadius = 12f;//Vanilla 8

			SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTwinRocket", "projectilePrefab", rocketPrefab);
        }

		private void AddPanicShield()
        {
			MegaDronePanicShield.shockEffectPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/lightningstakenova"), "RiskyModMegaDronePanicShieldEffect", false);

			EffectComponent ec = MegaDronePanicShield.shockEffectPrefab.GetComponent<EffectComponent>();
			ec.soundName = "Play_item_proc_deathMark";

			Content.Content.effectDefs.Add(new EffectDef(MegaDronePanicShield.shockEffectPrefab));

			AllyPrefabs.MegaDrone.AddComponent<MegaDronePanicShield>();
		}
	}
}
