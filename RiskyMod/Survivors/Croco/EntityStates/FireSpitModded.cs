using RiskyMod.Survivors.Croco;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Croco
{
	//Need to copypaste vanilla code to prevent weirdness with SpikeStrip
	public class FireSpitModded : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.crocoDamageTypeController = base.GetComponent<CrocoDamageTypeController>();
			Ray aimRay = base.GetAimRay();
			this.duration = FireSpitModded.baseDuration / this.attackSpeedStat;
			base.StartAimMode(this.duration + 2f, false);
			base.PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", this.duration);
			Util.PlaySound(FireSpitModded.attackString, base.gameObject);
			base.AddRecoil(-1f * FireSpitModded.recoilAmplitude, -1.5f * FireSpitModded.recoilAmplitude, -0.25f * FireSpitModded.recoilAmplitude, 0.25f * FireSpitModded.recoilAmplitude);
			base.characterBody.AddSpreadBloom(FireSpitModded.bloom);
			string muzzleName = "MouthMuzzle";
			if (FireSpitModded.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireSpitModded.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				bool hasDeeprot = CrocoCore.HasDeeprot(base.skillLocator);
				DamageType value = this.crocoDamageTypeController ? this.crocoDamageTypeController.GetDamageType().damageType : DamageType.Generic;
				if (!CrocoCore.HasDeeprot(base.skillLocator))
                {
					value = DamageType.Generic;
                }

				FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
				fireProjectileInfo.projectilePrefab = hasDeeprot ? projectilePrefabVanilla : projectilePrefab;
				fireProjectileInfo.position = aimRay.origin;
				fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
				fireProjectileInfo.owner = base.gameObject;
				fireProjectileInfo.damage = this.damageStat * FireSpitModded.damageCoefficient;
				fireProjectileInfo.damageTypeOverride = new DamageType?(value);
				fireProjectileInfo.force = FireSpitModded.force;
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

		public static GameObject projectilePrefabVanilla = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocospit");	//Behaves like Vanilla
		public static GameObject projectilePrefab;	//Has custom Blight6s damagetype
		public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/MuzzleflashCroco.prefab").WaitForCompletion();
		public static float baseDuration = 0.5f;
		public static float damageCoefficient = 2.4f;
		public static float force = 500f;
		public static string attackString = "Play_acrid_m2_shoot";
		public static float recoilAmplitude = 1f;
		public static float bloom = 0.3f;

		private float duration;
		private CrocoDamageTypeController crocoDamageTypeController;
	}
}
