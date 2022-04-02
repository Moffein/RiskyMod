using RoR2;
using RoR2.Skills;
using RoR2.UI;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Mage.Weapon
{
	public class PrepIceWall : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PrepIceWall.baseDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.duration + 2f);
			base.PlayAnimation("Gesture, Additive", "PrepIceWall", "PrepIceWall.playbackRate", this.duration);
			Util.PlaySound(PrepIceWall.PrepIceWallSoundString, base.gameObject);

			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(PrepIceWall.areaIndicatorPrefab);

			this.areaIndicatorCircleInstance = UnityEngine.Object.Instantiate<GameObject>(PrepIceWall.areaIndicatorCirclePrefab);
			if (areaIndicatorCircleInstance)
			{
				areaIndicatorCircleInstance.transform.localScale = blastRadius * Vector3.one;
			}

			this.UpdateAreaIndicator();
		}

		private void UpdateAreaIndicator()
		{
			bool flag = this.goodPlacement;
			this.goodPlacement = false;
			this.areaIndicatorInstance.SetActive(true);
			if (this.areaIndicatorInstance)
			{
				float num = PrepIceWall.maxDistance;
				float num2 = 0f;
				Ray aimRay = base.GetAimRay();
				RaycastHit raycastHit;
				if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimRay, base.gameObject, out num2), out raycastHit, num + num2, LayerIndex.world.mask))
				{
					this.areaIndicatorInstance.transform.position = raycastHit.point;
					this.areaIndicatorInstance.transform.up = raycastHit.normal;
					this.areaIndicatorInstance.transform.forward = -aimRay.direction;
					this.goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < PrepIceWall.maxSlopeAngle);
				}
				if (flag != this.goodPlacement || this.crosshairOverrideRequest == null)
				{
					CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
					if (overrideRequest != null)
					{
						overrideRequest.Dispose();
					}
					GameObject crosshairPrefab = this.goodPlacement ? PrepIceWall.goodCrosshairPrefab : PrepIceWall.badCrosshairPrefab;
					this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairPrefab, CrosshairUtils.OverridePriority.Skill);
				}
			}

			if (this.areaIndicatorCircleInstance)
			{
				Ray aimRay = base.GetAimRay();
				Vector3 aimPos = aimRay.origin + aimRay.direction * maxDistance;
				RaycastHit raycastHit = default(RaycastHit);
				if (Util.CharacterRaycast(base.gameObject, aimRay, out raycastHit, maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
				{
					aimPos = raycastHit.point;
				}

				this.areaIndicatorCircleInstance.transform.SetPositionAndRotation(aimPos, Quaternion.identity);
				areaIndicatorCircleInstance.transform.localScale = blastRadius * Vector3.one;
			}

			this.areaIndicatorInstance.SetActive(this.goodPlacement);
			this.areaIndicatorCircleInstance.SetActive(!this.goodPlacement);
		}

		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
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

		// Token: 0x06000C12 RID: 3090 RVA: 0x00064D98 File Offset: 0x00062F98
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				this.PlayAnimation("Gesture, Additive", "FireWall");
				Util.PlaySound(PrepIceWall.fireSoundString, base.gameObject);
				EffectManager.SimpleMuzzleFlash(PrepIceWall.muzzleflashEffect, base.gameObject, "MuzzleLeft", true);
				EffectManager.SimpleMuzzleFlash(PrepIceWall.muzzleflashEffect, base.gameObject, "MuzzleRight", true);
				if (this.goodPlacement)
				{
					if (this.areaIndicatorInstance && base.isAuthority)
					{
						Vector3 forward = this.areaIndicatorInstance.transform.forward;
						forward.y = 0f;
						forward.Normalize();
						Vector3 vector = Vector3.Cross(Vector3.up, forward);
						bool crit = Util.CheckRoll(this.critStat, base.characterBody.master);
						ProjectileManager.instance.FireProjectile(PrepIceWall.projectilePrefab, this.areaIndicatorInstance.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(vector), base.gameObject, this.damageStat * PrepIceWall.damageCoefficient, 0f, crit, DamageColorIndex.Default, null, -1f);
						ProjectileManager.instance.FireProjectile(PrepIceWall.projectilePrefab, this.areaIndicatorInstance.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(-vector), base.gameObject, this.damageStat * PrepIceWall.damageCoefficient, 0f, crit, DamageColorIndex.Default, null, -1f);
					}
				}
			}
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			EntityState.Destroy(this.areaIndicatorCircleInstance.gameObject);
			CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
			if (overrideRequest != null)
			{
				overrideRequest.Dispose();
			}

			//Always fire a blast attack so that Airborne enemies can be hit.
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
			effectData.scale = blastRadius;
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
				damageType = DamageType.Freeze2s,
				attackerFiltering = AttackerFiltering.NeverHitSelf,
				impactEffect = EffectCatalog.FindEffectIndexFromPrefab(hitEffectPrefab)
            };
			blastAttack.Fire();

			if (base.characterMotor && !base.characterMotor.isGrounded && base.characterBody)
            {
				Vector3 characterPos = base.characterBody.corePosition + Vector3.up;	//Need to shift position up a bit to get it to feel consistent.
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
		public static GameObject areaIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/FirewallAreaIndicator.prefab").WaitForCompletion();
		public static GameObject areaIndicatorCirclePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRainIndicator.prefab").WaitForCompletion();
		public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallWalkerProjectile.prefab").WaitForCompletion();
		public static float damageCoefficient = 1f;
		public static GameObject muzzleflashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageIceLarge.prefab").WaitForCompletion();
		public static GameObject goodCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion();
		public static GameObject badCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion();//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/BadCrosshair.prefab").WaitForCompletion();
		public static string PrepIceWallSoundString = "Play_mage_shift_start";
		public static float maxDistance = 600f;
		public static string fireSoundString = "Play_mage_shift_stop";
		public static float maxSlopeAngle = 70f;

		//for the guaranteed blastAttack
		public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIceExplosion.prefab").WaitForCompletion();
		public static GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXFrozen.prefab").WaitForCompletion();

		public static float blastRadius = 6f;//Ion Surge is 14f
		public static float blastJumpRadius = 14f;

		public static float blastForce = 3000f;
		public static Vector3 additionalForce = 400f * Vector3.up;

		private float duration;
		private float stopwatch;
		private bool goodPlacement;
		private GameObject areaIndicatorInstance;
		private GameObject areaIndicatorCircleInstance;
		private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
	}
}
