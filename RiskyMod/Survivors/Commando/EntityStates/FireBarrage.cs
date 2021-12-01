using RoR2;
using UnityEngine;
using R2API;
using RiskyMod.Survivors.Commando;

namespace EntityStates.RiskyMod.Commando
{
	public class FireBarrage : BaseState
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
			this.duration = FireBarrage.totalDuration;
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
				if (FireBarrage.effectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, muzzleName, false);
				}
				base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
			}
			base.AddRecoil(-0.8f * FireBarrage.recoilAmplitude, -1f * FireBarrage.recoilAmplitude, -0.1f * FireBarrage.recoilAmplitude, 0.15f * FireBarrage.recoilAmplitude);
			if (base.isAuthority)
			{
				BulletAttack ba = new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireBarrage.minSpread,
					maxSpread = FireBarrage.maxSpread,
					bulletCount = 1u,
					damage = FireBarrage.damageCoefficient * this.damageStat,
					force = FireBarrage.force,
					tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireBarrage.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					radius = FireBarrage.bulletRadius,
					smartCollision = true,
					damageType = DamageType.Stun1s,
					falloffModel = BulletAttack.FalloffModel.None
				};
				AddDamageType(ba);
				ba.Fire();
			}

			base.characterBody.AddSpreadBloom(FireBarrage.spreadBloomValue);
			this.totalBulletsFired++;
			Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);
		}

		public virtual void AddDamageType(BulletAttack ba)
        {
			DamageAPI.AddModdedDamageType(ba, CommandoCore.SuppressiveFireDamage);
		}

		public override void OnExit()
		{
			base.OnExit();
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
			return InterruptPriority.Skill;
		}

		public float internalBaseBulletCount;
		public float internalBaseDurationBetweenShots;


		public static GameObject explosionEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick");
		public static float blastRadius = 3f;
		public static float blastDamageCoefficient = 0.3f;	//Multiply by damage coefficient

		public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashBarrage");
		public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoBarrage");
		public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/TracerCommandoBoost");
		public static float damageCoefficient = 1f;
		public static float force = 100f;
		public static float minSpread = 0f;
		public static float maxSpread = 1f;
		public static float baseDurationBetweenShots = 0.12f;
		public static float totalDuration = 1f;
		public static float bulletRadius = 1.5f;
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

		public static void SuppressiveFireAOE(GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
		{
			if (damageInfo.HasModdedDamageType(CommandoCore.SuppressiveFireDamage))
			{
				EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData
				{
					origin = damageInfo.position,
					scale = blastRadius,
					rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
				}, true);
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.position = damageInfo.position;
				blastAttack.baseDamage = damageInfo.damage * blastDamageCoefficient;
				blastAttack.baseForce = 0f;
				blastAttack.radius = blastRadius;
				blastAttack.attacker = damageInfo.attacker;
				blastAttack.inflictor = null;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.crit = damageInfo.crit;
				blastAttack.procChainMask = damageInfo.procChainMask;
				blastAttack.procCoefficient = 0.5f;
				blastAttack.damageColorIndex = DamageColorIndex.Item;
				blastAttack.falloffModel = BlastAttack.FalloffModel.None;
				blastAttack.damageType = damageInfo.damageType;
				blastAttack.Fire();
			}
		}
	}
}
