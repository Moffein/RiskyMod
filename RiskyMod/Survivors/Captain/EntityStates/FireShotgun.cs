using RoR2;
using UnityEngine;
using EntityStates;
using BepInEx.Configuration;
using UnityEngine.AddressableAssets;
using System;

namespace EntityStates.RiskyMod.Captain
{
	public class FireShotgun : GenericBulletBaseState
	{
		public static ConfigEntry<bool> scalePellets;

		public static GameObject _muzzleFlashPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/Muzzleflash1.prefab").WaitForCompletion();
        public static GameObject _tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/TracerCaptainShotgun.prefab").WaitForCompletion();
        public static GameObject _hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/HitsparkCaptainShotgun.prefab").WaitForCompletion();
		public static int maxPellets = 21;


        public static float tightSoundSwitchThreshold = 2f;
        public static string wideSoundString = "Play_captain_m1_shootWide";
        public static string tightSoundString = "Play_captain_m1_shotgun_shootTight";

        private bool tight = false;

        private void SetStats()
		{
			this.baseDuration = 0.9f;
			this.bulletCount = 7;
			this.maxDistance = 2000f;
			this.bulletRadius = 0.3f;
			this.useSmartCollision = true;
			this.damageCoefficient = 1.1f;
			this.procCoefficient = 0.75f;
			this.force = 500f;
			this.minSpread = 0f;
			this.maxSpread = 0f;
			this.spreadPitchScale = 1f;
			this.spreadYawScale = 1f;
			this.spreadBloomValue = -0.2f;
			this.recoilAmplitudeY = 4f;
			this.recoilAmplitudeX = 2f;
			this.muzzleName = "MuzzleGun";
			this.muzzleFlashPrefab = _muzzleFlashPrefab;
			this.tracerEffectPrefab = _tracerEffectPrefab;
			this.hitEffectPrefab = _hitEffectPrefab;
		}

		public override void OnEnter()
		{
			tight = base.characterBody.spreadBloomAngle <= tightSoundSwitchThreshold;
			this.fireSoundString = (tight ? tightSoundString : wideSoundString);
			SetStats();
			base.OnEnter();
			base.PlayAnimation("Gesture, Additive", "FireCaptainShotgun");
			base.PlayAnimation("Gesture, Override", "FireCaptainShotgun");
            this.duration = scalePellets.Value ? this.baseDuration : this.baseDuration/this.attackSpeedStat;
        }

		public override void ModifyBullet(BulletAttack bulletAttack)
		{
			base.ModifyBullet(bulletAttack);
			bulletAttack.minSpread = 0f;    //Needs to be 0 or else weird things happen. Why isn't this vanilla?
			bulletAttack.damageType.damageSource = DamageSource.Primary;

			if (scalePellets.Value)
            {
                int bulletCountProcess = Mathf.FloorToInt((this.bulletCount * Mathf.Max(this.attackSpeedStat, 1f)));
                bulletAttack.bulletCount = (uint)bulletCountProcess;
                bulletAttack.force = this.force * Mathf.Sqrt(bulletCount / 7f);

				if (bulletCountProcess > maxPellets)
                {
                    int pelletsRemaining = bulletCountProcess - maxPellets;
                    bulletCountProcess = maxPellets;
                    bulletAttack.bulletCount = (uint)bulletCountProcess;
					if (isAuthority && inputBank)
					{
						var comp = gameObject.AddComponent<JustFireTheDamnBullets>();
						comp.damagePerBullet = 1.1f * damageStat;
						comp.useFalloff = tight;
						comp.force = bulletAttack.force;
						comp.spread = characterBody.spreadBloomAngle;
						comp.inputBank = inputBank;
						comp.pelletsRemaining = pelletsRemaining;
                    }
				}
            }

			if (tight)
			{
				bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
			}
			else
			{
				bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
			}
		}

        public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}

	//Only add this as Authority
	public class JustFireTheDamnBullets : MonoBehaviour
	{
		public float spread;
		public float force;
		public float damagePerBullet;
		public int pelletsRemaining;
		public InputBankTest inputBank;
		public bool useFalloff;

		private float age = 0f;

		private void FixedUpdate()
		{
			age += Time.fixedDeltaTime;
			if (age > 0f)
			{
				FireTheDamnBullets();
            }

            if (pelletsRemaining <= 0)
            {
                Destroy(this);
                return;
            }
        }

		private void FireTheDamnBullets()
        {
            if (!inputBank)
            {
                Destroy(this);
				return;
			}

			Ray aimRay = inputBank.GetAimRay();

            int pelletsToShoot = pelletsRemaining;
            if (pelletsRemaining > FireShotgun.maxPellets)
            {
                pelletsToShoot = FireShotgun.maxPellets;
                pelletsRemaining -= FireShotgun.maxPellets;
            }
			else
			{
				pelletsRemaining = 0;
			}

            new BulletAttack()
            {
                owner = gameObject,
                bulletCount = (uint)pelletsToShoot,
                maxDistance = 2000f,
                radius = 0.3f,
                smartCollision = true,
                damage = damagePerBullet,
                procCoefficient = 0.75f,
                procChainMask = default,
                falloffModel = useFalloff ? BulletAttack.FalloffModel.DefaultBullet : BulletAttack.FalloffModel.None,
                minSpread = 0f,
                maxSpread = useFalloff ? spread : 0f,
                damageType = DamageTypeCombo.GenericPrimary,
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                force = force,
                tracerEffectPrefab = FireShotgun._tracerEffectPrefab,
                hitEffectPrefab = FireShotgun._hitEffectPrefab,
                muzzleName = "MuzzleGun"
            }.Fire();
        }
	}
}