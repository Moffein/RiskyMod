using RoR2;
using RoR2.Skills;
using RoR2.UI;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RiskyMod;

namespace EntityStates.RiskyMod.Mage.Weapon
{
    public class PrepIonSurge : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepIonSurge.baseDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.duration + 2f);
			this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
			base.PlayAnimation("Gesture, Additive", "PrepIonSurge", "PrepIonSurge.playbackRate", this.duration);
			Util.PlaySound(PrepIonSurge.PrepIonSurgeSoundString, base.gameObject);

			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(PrepIonSurge.areaIndicatorPrefab);
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
				Util.PlaySound(PrepIonSurge.fireSoundString, base.gameObject);
				EffectManager.SimpleMuzzleFlash(PrepIonSurge.muzzleflashEffect, base.gameObject, "MuzzleLeft", true);
				EffectManager.SimpleMuzzleFlash(PrepIonSurge.muzzleflashEffect, base.gameObject, "MuzzleRight", true);
			}
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
			if (overrideRequest != null)
			{
				overrideRequest.Dispose();
			}

			if (base.isAuthority) FireBlastJump();

			base.OnExit();
		}

		//This runs clientside
		public void FireBlastJump()
		{
			Ray aimRay = base.GetAimRay();
			Vector3 aimPos = aimRay.origin + aimRay.direction * maxDistance;
			RaycastHit raycastHit = default(RaycastHit);
			if (Util.CharacterRaycast(base.gameObject, aimRay, out raycastHit, maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
            {
				aimPos = raycastHit.point;
			}

			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(Vector3.up);
			effectData.origin = aimPos;
			EffectManager.SpawnEffect(blastEffectPrefab, effectData, true);

			BlastAttack blastAttack = new BlastAttack
			{
				radius = blastRadius,
				procCoefficient = 1f,
				position = aimPos,
				attacker = base.gameObject,
				crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
				baseDamage = base.characterBody.damage * damageCoefficient,
				falloffModel = BlastAttack.FalloffModel.None,
				baseForce = 0f,
				teamIndex = base.GetTeam(),
				damageType = DamageType.Shock5s,
				attackerFiltering = AttackerFiltering.NeverHitSelf
			};
			blastAttack.AddModdedDamageType(SharedDamageTypes.Slow50For5s);
			blastAttack.Fire();

			if (base.characterMotor && !base.characterMotor.isGrounded && base.characterBody)
			{
				Vector3 characterPos = base.characterBody.corePosition + Vector3.up;    //Need to shift position up a bit to get it to feel consistent.
				Vector3 diff = characterPos - aimPos;
				float distSqr = diff.sqrMagnitude;
				if (distSqr <= blastJumpRadius * blastJumpRadius)
				{
					base.characterMotor.ApplyForce(blastForce * diff.normalized, true, false);
				}
			}
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x000058E2 File Offset: 0x00003AE2
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}

		public static float baseDuration = 0.5f;
		public static float damageCoefficient = 8f;
		public static GameObject areaIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRainIndicator.prefab").WaitForCompletion();
		public static GameObject muzzleflashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightningLarge.prefab").WaitForCompletion();
		public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion();
		public static string PrepIonSurgeSoundString = "Play_mage_m1_cast_lightning";
		public static float maxDistance = 600f;
		public static string fireSoundString = "Play_mage_R_lightningBlast";

		//for the guaranteed blastAttack
		public static GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/OmniImpactVFXLightningMage.prefab").WaitForCompletion();

		public static float blastRadius = 14f;//Ion Surge is 14f
		public static float blastJumpRadius = 14f;

		public static float blastForce = 3000f;
		public static Vector3 additionalForce = 400f * Vector3.up;

		private float duration;
		private float stopwatch;
		private GameObject areaIndicatorInstance;
		private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
	}
}
