using RoR2;
using UnityEngine;
using R2API;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors;

namespace EntityStates.RiskyMod.Loader
{
    public class GroundSlamScaled : BaseCharacterMain
    {
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Body", "GroundSlam", 0.2f);
			if (base.isAuthority)
			{
				base.characterMotor.onMovementHit += this.OnMovementHit;
				base.characterMotor.velocity.y = GroundSlamScaled.initialVerticalVelocity;
			}
			Util.PlaySound(GroundSlamScaled.enterSoundString, base.gameObject);
			this.previousAirControl = base.characterMotor.airControl;
			base.characterMotor.airControl = GroundSlamScaled.airControl;
			this.leftFistEffectInstance = UnityEngine.Object.Instantiate<GameObject>(GroundSlamScaled.fistEffectPrefab, base.FindModelChild("MechHandR"));
			this.rightFistEffectInstance = UnityEngine.Object.Instantiate<GameObject>(GroundSlamScaled.fistEffectPrefab, base.FindModelChild("MechHandL"));

			if (base.characterBody)
            {
				initialY = base.characterBody.footPosition.y;
            }
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.characterMotor)
			{
				base.characterMotor.moveDirection = base.inputBank.moveVector;
				base.characterDirection.moveVector = base.characterMotor.moveDirection;
				CharacterMotor characterMotor = base.characterMotor;
				characterMotor.velocity.y = characterMotor.velocity.y + GroundSlamScaled.verticalAcceleration * Time.fixedDeltaTime;
				if (base.fixedAge >= GroundSlamScaled.minimumDuration && (this.detonateNextFrame || base.characterMotor.Motor.GroundingStatus.IsStableOnGround))
				{
					BlastAttack.Result result = this.DetonateAuthority();
					if (result.hitCount > 0)
                    {
						if (base.healthComponent)
						{
							base.healthComponent.AddBarrierAuthority(EntityStates.Loader.LoaderMeleeAttack.barrierPercentagePerHit * base.healthComponent.fullBarrier);
						}
					}
					this.outer.SetNextStateToMain();
				}

				//Increase slam hitbox size
				if (!(detonateNextFrame || base.characterMotor.Motor.GroundingStatus.IsStableOnGround)
					&& base.fixedAge >= GroundSlamScaled.minimumDuration)
				{
					int potentialHits = SneedUtils.SneedUtils.FindEnemiesInSphere(5f, base.characterBody.footPosition, base.GetTeam());
					if (potentialHits > 0)
					{
						detonateNextFrame = true;
					}
				}
			}
		}

		public override void OnExit()
		{
			if (base.isAuthority)
			{
				base.characterMotor.onMovementHit -= this.OnMovementHit;
				base.characterMotor.Motor.ForceUnground();
				base.characterMotor.velocity *= GroundSlamScaled.exitSlowdownCoefficient;
				base.characterMotor.velocity.y = GroundSlamScaled.exitVerticalVelocity;
			}
			base.characterMotor.airControl = this.previousAirControl;
			EntityState.Destroy(this.leftFistEffectInstance);
			EntityState.Destroy(this.rightFistEffectInstance);
			base.OnExit();
		}

		private void OnMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
		{
			this.detonateNextFrame = true;
		}

		public BlastAttack.Result DetonateAuthority()
		{
			Vector3 footPosition = base.characterBody.footPosition;
			EffectManager.SpawnEffect(GroundSlamScaled.blastEffectPrefab, new EffectData
			{
				origin = footPosition,
				scale = GroundSlamScaled.blastRadius
			}, true);
			BlastAttack ba = new BlastAttack
			{
				attacker = base.gameObject,
				baseDamage = this.damageStat * GroundSlamScaled.blastDamageCoefficient,
				baseForce = 1200f,
				bonusForce = new Vector3(0f, 3000f, 0f),
				crit = base.RollCrit(),
				damageType = DamageType.Stun1s,
				falloffModel = BlastAttack.FalloffModel.None,
				procCoefficient = GroundSlamScaled.blastProcCoefficient,
				radius = GroundSlamScaled.blastRadius,
				position = footPosition,
				attackerFiltering = AttackerFiltering.NeverHit,
				impactEffect = EffectCatalog.FindEffectIndexFromPrefab(GroundSlamScaled.blastImpactEffectPrefab),
				teamIndex = base.teamComponent.teamIndex
			};

			if (base.characterBody)
			{
				float percent = (initialY - base.characterBody.footPosition.y) / maxBoostDistance;
				float damageBoostCoefficient = Mathf.Lerp(1f, maxDamageBoost, percent);
				float radiusBoostCoefficient = Mathf.Lerp(1f, maxRadiusBoost, percent);

				ba.baseDamage *= damageBoostCoefficient;
				ba.radius *= radiusBoostCoefficient;
			}
			ba.AddModdedDamageType(SharedDamageTypes.AntiFlyingForce);

			return ba.Fire();
		}

		public static float airControl = 0.2f;
		public static float minimumDuration = 0.1f;
		public static float blastRadius = 10f;
		public static float blastProcCoefficient = 1f;
		public static float blastDamageCoefficient = 16f;
		public static string enterSoundString = "Play_loader_R_variant_whooshDown";
		public static float initialVerticalVelocity = -20f;
		public static float exitVerticalVelocity = 12f;
		public static float verticalAcceleration = -100f;
		public static float exitSlowdownCoefficient = 0.2f;
		public static GameObject blastImpactEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfxloaderlightning");
		public static GameObject blastEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/loadergroundslam");
		public static GameObject fistEffectPrefab;

		public static float maxDamageBoost = 3f;
		public static float maxRadiusBoost = 3f;
		public static float maxForceBoost = 3f;
		public static float maxBoostDistance = 200f;

		private float initialY;
		private float previousAirControl;
		private GameObject leftFistEffectInstance;
		private GameObject rightFistEffectInstance;
		private bool detonateNextFrame;
	}
}
