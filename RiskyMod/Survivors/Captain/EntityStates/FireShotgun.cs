﻿using RoR2;
using UnityEngine;
using EntityStates;
using BepInEx.Configuration;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Captain
{
	public class FireShotgun : GenericBulletBaseState
	{
		public static ConfigEntry<bool> scalePellets;

		public static GameObject _muzzleFlashPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/Muzzleflash1.prefab").WaitForCompletion();
        public static GameObject _tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/TracerCaptainShotgun.prefab").WaitForCompletion();
        public static GameObject _hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/HitsparkCaptainShotgun.prefab").WaitForCompletion();

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
                bulletAttack.bulletCount = (uint)Mathf.FloorToInt((this.bulletCount * Mathf.Max(this.attackSpeedStat, 1f)));
                bulletAttack.force = this.force * Mathf.Sqrt(bulletCount / 7f);
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

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static float tightSoundSwitchThreshold = 2f;
		public static string wideSoundString = "Play_captain_m1_shootWide";
		public static string tightSoundString = "Play_captain_m1_shotgun_shootTight";

		private bool tight = false;
	}
}