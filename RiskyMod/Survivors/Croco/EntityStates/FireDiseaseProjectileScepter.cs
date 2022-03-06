using RoR2;
using UnityEngine;
using R2API;
using RoR2.Projectile;

namespace EntityStates.RiskyMod.Croco
{
	public class FireDiseaseProjectileScepter : BaseState
	{
		// Token: 0x06004470 RID: 17520 RVA: 0x00113E24 File Offset: 0x00112024
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			this.duration = baseDuration / this.attackSpeedStat;
			base.StartAimMode(this.duration + 2f, false);
			base.PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", this.duration);
			Util.PlaySound(attackString, base.gameObject);
			base.AddRecoil(-1f * recoilAmplitude, -1.5f * recoilAmplitude, -0.25f * recoilAmplitude, 0.25f * recoilAmplitude);
			base.characterBody.AddSpreadBloom(bloom);
			string muzzleName = "MouthMuzzle";
			if (effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
				fireProjectileInfo.projectilePrefab = projectilePrefab;
				fireProjectileInfo.position = aimRay.origin;
				fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
				fireProjectileInfo.owner = base.gameObject;
				fireProjectileInfo.damage = this.damageStat * damageCoefficient;
				fireProjectileInfo.force = force;
				fireProjectileInfo.crit = Util.CheckRoll(this.critStat, base.characterBody.master);

				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
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
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static GameObject projectilePrefab;
		public static GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashcroco");
		public static float baseDuration = 0.5f;
		public static float damageCoefficient = 1f;
		public static float force = 0f;
		public static string attackString = "Play_acrid_R_shoot";
		public static float recoilAmplitude = 2f;
		public static float bloom = 1f;

		private float duration;
	}
}
