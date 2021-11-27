using R2API;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using UnityEngine;
namespace EntityStates.RiskyMod.Bandit2.Revolver
{
	public class FireLightsOut : BaseSidearmState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			base.AddRecoil(-3f * recoilAmplitude, -4f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzlePistol";
			Util.PlaySound(attackSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", this.duration);
			if (effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				BulletAttack bulletAttack = new BulletAttack();
				bulletAttack.owner = base.gameObject;
				bulletAttack.weapon = base.gameObject;
				bulletAttack.origin = aimRay.origin;
				bulletAttack.aimVector = aimRay.direction;
				bulletAttack.minSpread = minSpread;
				bulletAttack.maxSpread = maxSpread;
				bulletAttack.bulletCount = 1u;
				bulletAttack.damage = damageCoefficient * this.damageStat;
				bulletAttack.force = force;
				bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
				bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
				bulletAttack.muzzleName = muzzleName;
				bulletAttack.hitEffectPrefab = hitEffectPrefab;
				bulletAttack.isCrit = base.RollCrit();
				bulletAttack.HitEffectNormal = false;
				bulletAttack.radius = bulletRadius;
				bulletAttack.damageType |= DamageType.BonusToLowHealth;
				bulletAttack.smartCollision = true;
				bulletAttack.maxDistance = 1000f;

				SpecialDamageController sdc = base.GetComponent<SpecialDamageController>();
				if (sdc)
                {
					bulletAttack.damageType |= sdc.GetDamageType();
				}
				DamageAPI.AddModdedDamageType(bulletAttack, Bandit2Core.SpecialDamage);

				bulletAttack.Fire();
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new ExitSidearm());
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

        public override void LoadStats()
        {
			crosshairOverridePrefab = _crosshairOverridePrefab;
			baseDuration = 1f;
        }

        public static GameObject _crosshairOverridePrefab = Resources.Load<GameObject>("prefabs/crosshair/Bandit2CrosshairPrepRevolverFire");

		public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashBandit2");
		public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/HitsparkBandit2Pistol");
		public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/TracerBanditPistol");

		public static float damageCoefficient = 6f;
		public static float force = 2000f;	//vanilla is 1500f

		public static float minSpread = 0f;
		public static float maxSpread = 0f;

		public static string attackSoundString = "Play_bandit2_R_fire";
		public static float recoilAmplitude  =1f;
		public static float bulletRadius = 1f;
	}
}
