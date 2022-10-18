using RoR2;
using RoR2.Skills;
using RoR2.UI;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RiskyMod;
using RoR2.Projectile;

namespace EntityStates.RiskyMod.Mage.Weapon
{
    public class PrepFireStorm : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepFireStorm.baseDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.duration + 2f);
			this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
			base.PlayAnimation("Gesture, Additive", "PrepIonSurge", "PrepIonSurge.playbackRate", this.duration);
			Util.PlaySound(PrepFireStorm.PrepIonSurgeSoundString, base.gameObject);

			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(PrepFireStorm.areaIndicatorPrefab);
			if (areaIndicatorInstance)
            {
				areaIndicatorInstance.transform.localScale = blastRadius * Vector3.one;
            }

			this.UpdateAreaIndicator();
		}

		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		private void UpdateAreaIndicator()
		{
			if (this.areaIndicatorInstance)
			{
				Ray aimRay = base.GetAimRay();
				Vector3 aimPos = aimRay.origin + aimRay.direction * maxDistance;
				RaycastHit raycastHit = default(RaycastHit);
				if (Util.CharacterRaycast(base.gameObject, aimRay, out raycastHit, maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
				{
					aimPos = raycastHit.point;
				}

				this.areaIndicatorInstance.transform.SetPositionAndRotation(aimPos, Quaternion.identity);
				areaIndicatorInstance.transform.localScale = blastRadius * Vector3.one;
			}
		}

		private void OnPreRenderSceneCam(SceneCamera sceneCam)
		{
			if (this.areaIndicatorInstance)
			{
				this.areaIndicatorInstance.transform.gameObject.layer = ((sceneCam.cameraRigController.target == base.gameObject) ? LayerIndex.defaultLayer.intVal : LayerIndex.noDraw.intVal);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (!base.inputBank.skill3.down && base.isAuthority)//this.stopwatch >= this.duration && 
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.PlayAnimation("Gesture, Additive", "FireWall");
				Util.PlaySound(PrepFireStorm.fireSoundString, base.gameObject);
				EffectManager.SimpleMuzzleFlash(PrepFireStorm.muzzleflashEffect, base.gameObject, "MuzzleLeft", true);
				EffectManager.SimpleMuzzleFlash(PrepFireStorm.muzzleflashEffect, base.gameObject, "MuzzleRight", true);
			}
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
			if (overrideRequest != null)
			{
				overrideRequest.Dispose();
			}

			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				Vector3 aimPos = aimRay.origin + aimRay.direction * maxDistance;
				RaycastHit raycastHit = default(RaycastHit);
				if (Util.CharacterRaycast(base.gameObject, aimRay, out raycastHit, maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
				{
					aimPos = raycastHit.point;
				}

				if (base.characterMotor)
				{
					Vector3 force = -1f * aimRay.direction * blastForce;
					if (base.characterMotor.velocity.y < 0)
					{
						base.characterMotor.velocity.y = 0;
						if (aimRay.direction.y < 0f) force += additionalForce;
					}
					base.characterMotor.ApplyForce(force, true, false);
				}

				ProjectileManager.instance.FireProjectile(new FireProjectileInfo
				{
					damage = base.damageStat * PrepFireStorm.damageCoefficient,
					crit = base.RollCrit(),
					damageColorIndex = DamageColorIndex.Default,
					position = aimPos,
					procChainMask = default,
					force = 0f,
					owner = base.gameObject,
					projectilePrefab = PrepFireStorm.projectilePrefab,
					rotation = Quaternion.identity,
					speedOverride = 0f,
					target = null
				});
			}

			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		public static float baseDuration = 0.5f;
		public static float damageCoefficient = 21.6f;
		public static GameObject areaIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRainIndicator.prefab").WaitForCompletion();
		public static GameObject muzzleflashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MuzzleflashMageFireLarge.prefab").WaitForCompletion();
		public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion();
		public static string PrepIonSurgeSoundString = "Play_mage_m1_shoot";
		public static float maxDistance = 600f;
		public static string fireSoundString = "Play_mage_R_end";

		public static GameObject projectilePrefab;

		//for the guaranteed blastAttack
		public static GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion();

		public static float blastRadius = 8f;//Ion Surge is 14f
		public static float blastJumpRadius = 14f;

		public static float blastForce = 3200f; //4000f is railgunner
		public static Vector3 additionalForce = 200f * Vector3.up;

		private float duration;
		private float stopwatch;
		private GameObject areaIndicatorInstance;
		private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
	}
}
