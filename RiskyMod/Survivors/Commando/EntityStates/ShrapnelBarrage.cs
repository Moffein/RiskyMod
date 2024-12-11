using RoR2;
using UnityEngine;
using R2API;
using RiskyMod.Survivors.Commando;
using static RoR2.BulletAttack;

namespace EntityStates.RiskyMod.Commando
{
	public class ShrapnelBarrage : BaseState
	{
		public virtual void LoadStats()
		{
			internalBaseBulletCount = baseBulletCount;
			internalBaseDurationBetweenShots = baseDurationBetweenShots;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			LoadStats();
			base.characterBody.SetSpreadBloom(0.2f, false);
			this.duration = ShrapnelBarrage.totalDuration;
			this.durationBetweenShots = internalBaseDurationBetweenShots / this.attackSpeedStat;
			this.bulletCount = (int)((float)internalBaseBulletCount * this.attackSpeedStat);


			maxAttackSpeed = this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			base.PlayCrossfade("Gesture, Additive", "FireBarrage", "FireBarrage.playbackRate", this.duration, 0.2f);
			base.PlayCrossfade("Gesture, Override", "FireBarrage", "FireBarrage.playbackRate", this.duration, 0.2f);
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			this.FireBullet();
		}

		private void FireBullet()
		{
			Ray aimRay = base.GetAimRay();
			if (this.modelAnimator)
			{
				if (ShrapnelBarrage.effectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(ShrapnelBarrage.effectPrefab, base.gameObject, muzzleName, false);
				}
				base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
			}
			base.AddRecoil(-0.8f * ShrapnelBarrage.recoilAmplitude, -1f * ShrapnelBarrage.recoilAmplitude, -0.1f * ShrapnelBarrage.recoilAmplitude, 0.15f * ShrapnelBarrage.recoilAmplitude);
			if (base.isAuthority)
			{
				triggeredExplosion = false;
				var b = new BulletAttack
				{
					tracerEffectPrefab = ShrapnelBarrage.tracerEffectPrefab,
					damage = 0f,
					procCoefficient = 0f,
					damageType = DamageType.Silent | DamageType.NonLethal,
					owner = base.gameObject,
					aimVector = aimRay.direction,
					isCrit = false,
					minSpread = minSpread,
					maxSpread = maxSpread,
					origin = aimRay.origin,
					maxDistance = 2000f,
					muzzleName = muzzleName,
					radius = bulletRadius,
					hitCallback = ComboHitCallback
				};
				b.damageType.damageSource = DamageSource.Special;
				b.Fire();
            }

			base.characterBody.AddSpreadBloom(ShrapnelBarrage.spreadBloomValue);
			this.totalBulletsFired++;
			Util.PlaySound(ShrapnelBarrage.fireBarrageSoundString, base.gameObject);
		}

        protected virtual bool ComboHitCallback(BulletAttack bulletRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && !triggeredExplosion && base.characterBody)
            {
                triggeredExplosion = true;

                Vector3 attackForce = bulletRef.aimVector != null ? force * bulletRef.aimVector.normalized : Vector3.zero;

                if (explosionEffectPrefab)
                {
                    EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData { origin = hitInfo.point, scale = blastRadius }, true);
                }

				var ba = new BlastAttack()
				{
					attacker = base.gameObject,
					attackerFiltering = AttackerFiltering.NeverHitSelf,
					baseDamage = this.damageStat * DamageCoefficient,
					baseForce = 0f,
					bonusForce = attackForce,
					canRejectForce = true,
					crit = base.RollCrit(),
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Stun1s,
					falloffModel = BlastAttack.FalloffModel.None,
					inflictor = base.gameObject,
					position = hitInfo.point,
					procChainMask = default,
					procCoefficient = 1f,
					radius = blastRadius,
					teamIndex = base.GetTeam()
				};
				ba.damageType.damageSource = DamageSource.Special;
				ba.Fire();
            }

            return false;
        }

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			//Fire rate updates during the skill.
			float currentAttackSpeed = this.attackSpeedStat;
			if (base.characterBody)
            {
				currentAttackSpeed = base.characterBody.attackSpeed;
            }
			this.durationBetweenShots = internalBaseDurationBetweenShots / maxAttackSpeed;
			if (maxAttackSpeed < currentAttackSpeed)
            {
				maxAttackSpeed = currentAttackSpeed;
				this.bulletCount = (int)((float)internalBaseBulletCount * maxAttackSpeed);
			}

			this.stopwatchBetweenShots += Time.fixedDeltaTime;
			if (this.stopwatchBetweenShots >= this.durationBetweenShots && this.totalBulletsFired < this.bulletCount)
			{
				this.stopwatchBetweenShots -= this.durationBetweenShots;
				this.FireBullet();
			}
			if (base.fixedAge >= this.duration && this.totalBulletsFired == this.bulletCount && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (base.inputBank && base.inputBank.skill4.down) return InterruptPriority.Pain;
			return InterruptPriority.Skill;
		}

		protected virtual float DamageCoefficient => 1.2f;

		private bool triggeredExplosion = false;
		public float internalBaseBulletCount;
		public float internalBaseDurationBetweenShots;


		public static GameObject explosionEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick");
		public static float blastRadius = 3f;

		public static GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashBarrage");
		public static GameObject hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoBarrage");
		public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/TracerCommandoBoost");
		public static float force = 100f;
		public static float minSpread = 0f;
		public static float maxSpread = 1f;
		public static float baseDurationBetweenShots = 0.12f;
		public static float totalDuration = 1f;
		public static float bulletRadius = 0.5f;
		public static int baseBulletCount = 6;
		public static string fireBarrageSoundString = "Play_commando_m1";
		public static float recoilAmplitude = 1.5f;
		public static float spreadBloomValue = 0.15f;
		public static string muzzleName = "MuzzleRight";

		private int totalBulletsFired;
		private int bulletCount;
		public float stopwatchBetweenShots;
		private Animator modelAnimator;
		private Transform modelTransform;
		private float duration;
		private float durationBetweenShots;

		private float maxAttackSpeed;
	}
}
