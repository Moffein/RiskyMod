using RoR2;
using UnityEngine;

namespace EntityStates.RiskyMod.Mage
{
    public class Flamethrower : BaseState
    {
		public virtual void LoadStats()
        {
			loadBaseTickCount = Flamethrower.baseTickCount;
			loadBaseTickFrequency = Flamethrower.baseTickFrequency;
			loadignitePercentChance = Flamethrower.ignitePercentChance;
			loadMaxDistance = Flamethrower.maxDistance;
        }

		public virtual void ModifyBullet(BulletAttack ba) { }

		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;

			LoadStats();

			this.entryDuration = Flamethrower.baseEntryDuration / this.attackSpeedStat;

			this.tickFrequency = loadBaseTickFrequency * this.attackSpeedStat;
			totalTickCount = Mathf.FloorToInt(loadBaseTickCount * this.attackSpeedStat);

			this.flamethrowerDuration = Flamethrower.baseFlamethrowerDuration;

			effectResetStopwatch = 0f;

			Transform modelTransform = base.GetModelTransform();
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.entryDuration + this.flamethrowerDuration + 1f);
			}
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				this.leftMuzzleTransform = this.childLocator.FindChild("MuzzleLeft");
				this.rightMuzzleTransform = this.childLocator.FindChild("MuzzleRight");
			}

			if (base.isAuthority && base.characterBody)
			{
				this.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			}
			base.PlayAnimation("Gesture, Additive", "PrepFlamethrower", "Flamethrower.playbackRate", this.entryDuration);
		}

		public override void OnExit()
		{
			Util.PlaySound(Flamethrower.endAttackSoundString, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "ExitFlamethrower", 0.1f);
			if (this.leftFlamethrowerTransform)
			{
				EntityState.Destroy(this.leftFlamethrowerTransform.gameObject);
			}
			if (this.rightFlamethrowerTransform)
			{
				EntityState.Destroy(this.rightFlamethrowerTransform.gameObject);
			}
			base.OnExit();
		}

		private void FireGauntlet(string muzzleString)
		{
			totalTickCount--;
			Ray aimRay = base.GetAimRay();
			if (base.isAuthority)
			{
				BulletAttack ba = new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = 0f,
					damage = Flamethrower.tickDamageCoefficient * this.damageStat,
					force = Flamethrower.force,
					muzzleName = muzzleString,
					hitEffectPrefab = Flamethrower.impactEffectPrefab,
					isCrit = this.isCrit,
					radius = Flamethrower.radius,
					falloffModel = BulletAttack.FalloffModel.None,
					stopperMask = LayerIndex.world.mask,
					procCoefficient = Flamethrower.procCoefficientPerTick,
					maxDistance = loadMaxDistance,
					smartCollision = true,
					damageType = (Util.CheckRoll(loadignitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic)
				};
				ModifyBullet(ba);
				ba.Fire();
				if (base.characterMotor)
				{
					base.characterMotor.ApplyForce(aimRay.direction * -Flamethrower.recoilForce, false, false);
				}
			}
		}

		public void InitFlamethrowerEffect()
        {
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild("MuzzleLeft");
				Transform transform2 = this.childLocator.FindChild("MuzzleRight");
				if (transform)
				{
					this.leftFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(Flamethrower.flamethrowerEffectPrefab, transform).transform;
				}
				if (transform2)
				{
					this.rightFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(Flamethrower.flamethrowerEffectPrefab, transform2).transform;
				}
				if (this.leftFlamethrowerTransform)
				{
					this.leftFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
				}
				if (this.rightFlamethrowerTransform)
				{
					this.rightFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
				}
			}
		}

		private void ResetFlamethrowerEffect()
        {
			if (this.leftFlamethrowerTransform)
			{
				EntityState.Destroy(this.leftFlamethrowerTransform.gameObject);
				this.leftFlamethrowerTransform = null;
			}
			if (this.rightFlamethrowerTransform)
			{
				EntityState.Destroy(this.rightFlamethrowerTransform.gameObject);
				this.rightFlamethrowerTransform = null;
			}
			InitFlamethrowerEffect();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;

			if (this.stopwatch >= this.entryDuration && !this.hasBegunFlamethrower)
			{
				this.hasBegunFlamethrower = true;
				Util.PlaySound(Flamethrower.startAttackSoundString, base.gameObject);
				base.PlayAnimation("Gesture, Additive", "Flamethrower", "Flamethrower.playbackRate", this.flamethrowerDuration);
				InitFlamethrowerEffect();
				this.FireGauntlet("MuzzleCenter");
			}
			if (this.hasBegunFlamethrower)
			{
				effectResetStopwatch += Time.fixedDeltaTime;
				this.flamethrowerStopwatch += Time.deltaTime;
				if (this.flamethrowerStopwatch > 1f / tickFrequency)
				{
					this.flamethrowerStopwatch -= 1f / tickFrequency;
					this.FireGauntlet("MuzzleCenter");
				}
				this.UpdateFlamethrowerEffect();
			}

			if (effectResetStopwatch > Flamethrower.effectResetDuration)
            {
				effectResetStopwatch = 0f;
				if (totalTickCount * Mathf.Max(1f / tickFrequency, Time.fixedDeltaTime) > 0.6f)
				{
					ResetFlamethrowerEffect();
				}
			}

			if (totalTickCount <= 0 && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		private void UpdateFlamethrowerEffect()
		{
			Ray aimRay = base.GetAimRay();
			Vector3 direction = aimRay.direction;
			Vector3 direction2 = aimRay.direction;
			if (this.leftFlamethrowerTransform)
			{
				this.leftFlamethrowerTransform.forward = direction;
			}
			if (this.rightFlamethrowerTransform)
			{
				this.rightFlamethrowerTransform.forward = direction2;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static GameObject flamethrowerEffectPrefab;
		public static GameObject impactEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx");
		public static GameObject tracerEffectPrefab = null;
		public static float maxDistance = 20f;
		public static float radius = 2f;
		public static float baseEntryDuration = 0.6f;
		public static float baseFlamethrowerDuration = 3f;
		public static float tickDamageCoefficient = 1f;
		public static float procCoefficientPerTick = 1f;
		public static float baseTickFrequency = 7f;
		public static float force = 100f;
		public static string startAttackSoundString = "Play_mage_R_start";
		public static string endAttackSoundString = "Play_mage_R_end";
		public static float ignitePercentChance = 50f;
		public static float recoilForce = 0f;

		public static float effectResetDuration = 2.5f;
		private float effectResetStopwatch;

		private static int baseTickCount = 20;
		private int totalTickCount;
		private float tickFrequency;

		public int loadBaseTickCount;
		public float loadBaseTickFrequency;
		public float loadignitePercentChance;
		public float loadMaxDistance;

		private float flamethrowerStopwatch;
		private float stopwatch;
		private float entryDuration;
		private float flamethrowerDuration;
		private bool hasBegunFlamethrower;
		private ChildLocator childLocator;
		private Transform leftFlamethrowerTransform;
		private Transform rightFlamethrowerTransform;
		private Transform leftMuzzleTransform;
		private Transform rightMuzzleTransform;
		private bool isCrit;
		private const float flamethrowerEffectBaseDistance = 16f;
	}
}
