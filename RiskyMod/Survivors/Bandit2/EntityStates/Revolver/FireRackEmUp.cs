using R2API;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using UnityEngine;
namespace EntityStates.RiskyMod.Bandit2.Revolver
{
	public class FireRackEmUp : BaseSidearmState
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
				bulletAttack.damage = damageCoefficient * this.damageStat * DesperadoRework.GetDesperadoMult(base.characterBody);
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
				bulletAttack.maxDistance = 200f;

				SpecialDamageController sdc = base.GetComponent<SpecialDamageController>();
				if (sdc)
				{
					bulletAttack.damageType |= sdc.GetDamageType();
				}
				DamageAPI.AddModdedDamageType(bulletAttack, Bandit2Core.SpecialDamage);
				DamageAPI.AddModdedDamageType(bulletAttack, Bandit2Core.RackEmUpDamage);

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
				if (bulletCount <= 0)
				{
					this.outer.SetNextState(new ExitSidearm());
				}
				else
                {
					this.outer.SetNextState(new FireRackEmUp { bulletCount = this.bulletCount });
                }
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public override void LoadStats()
		{
			crosshairOverridePrefab = _crosshairOverridePrefab;
			bulletCount--;

			if (bulletCount <= 0)
			{
				baseDuration = 1f;
			}
			else
            {
				baseDuration = 0.13f;
            }
		}

		public static GameObject _crosshairOverridePrefab = Resources.Load<GameObject>("prefabs/crosshair/Bandit2CrosshairPrepRevolverFire");

		public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashBandit2");
		public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/HitsparkBandit2Pistol");
		public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/TracerBanditPistol");

		public static float damageCoefficient = 1f;
		public static float bonusDamageCoefficient = 0.3f;
		public static float force = 300f;

		public static float minSpread = 0f;
		public static float maxSpread = 2.5f;

		public static string attackSoundString = "Play_bandit2_R_fire";
		public static float recoilAmplitude = 2.2f;
		public static float bulletRadius = 0.5f;

		public int bulletCount = 6;
	}
}
