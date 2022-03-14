using RoR2;
using UnityEngine;

namespace EntityStates.RiskyMod.Commando
{
	public class FirePhaseBlast : GenericBulletBaseState
	{
		public static GameObject _muzzleFlashPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashFMJ");
		public static GameObject _tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/TracerCommandoShotgun");
		public static GameObject _hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoShotgun");

		public static float delayBetweenShotgunBlasts = 0.2f;
		private bool hasFiredSecondBlast;

		private void LoadStats()
        {
			baseDuration = 0.5f;
			bulletCount = 4;
			maxDistance = 60f;
			bulletRadius = 3f;
			useSmartCollision = true;
			damageCoefficient = 2f;
			procCoefficient = 0.75f;
			force = 300f;
			minSpread = 0f;
			maxSpread = 8f;
			spreadPitchScale = 1f;
			spreadYawScale = 1f;
			spreadBloomValue = 100f;
			recoilAmplitudeY = 3f;
			recoilAmplitudeX = 2f;
			muzzleName = "MuzzleCenter";
			fireSoundString = "Play_commando_M2";
			muzzleFlashPrefab = _muzzleFlashPrefab;
			tracerEffectPrefab = _tracerEffectPrefab;
			hitEffectPrefab = _hitEffectPrefab;
		}

		public override void OnEnter()
		{
			LoadStats();
			hasFiredSecondBlast = false;
			this.muzzleName = "MuzzleLeft";
			base.OnEnter();
			base.PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
			base.PlayAnimation("Gesture Override, Left", "FirePistol, Left");
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.hasFiredSecondBlast && FirePhaseBlast.delayBetweenShotgunBlasts / this.attackSpeedStat < base.fixedAge)
			{
				this.hasFiredSecondBlast = true;
				base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
				base.PlayAnimation("Gesture Override, Right", "FirePistol, Right");
				this.muzzleName = "MuzzleRight";
				this.FireBullet(base.GetAimRay());
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
