﻿using R2API;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Bandit2.Revolver.Scepter
{
    class FireLightsOutScepter : BaseSidearmState
	{
		public override void OnEnter()
		{
			shotsFired++;
			base.OnEnter();
			base.AddRecoil(-3f * recoilAmplitude, -4f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzlePistol";
			Util.PlaySound(attackSoundString, base.gameObject);

			bool playAnim = true;
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Gesture, Additive");
				if (layerIndex >= 0)
				{
					AnimatorStateInfo animStateInfo = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex);
					if (animStateInfo.IsName("SlashBlade"))
					{
						playAnim = false;
					}
				}
			}
			if (playAnim) base.PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", this.duration);
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
				bulletAttack.damage = damageCoefficient * this.damageStat * PersistentDesperado.GetDesperadoMult(base.characterBody);
				bulletAttack.force = force;
				bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
				bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
				bulletAttack.muzzleName = muzzleName;
				bulletAttack.hitEffectPrefab = hitEffectPrefab;
				bulletAttack.isCrit = base.RollCrit();
				bulletAttack.HitEffectNormal = false;
				bulletAttack.radius = bulletRadius;
				bulletAttack.smartCollision = true;
				bulletAttack.maxDistance = 1000f;

				SpecialDamageController sdc = base.GetComponent<SpecialDamageController>();
				if (sdc)
				{
					SkillDef selectedPassive = sdc.GetPassiveSkillDef();

                    if (selectedPassive == Skills.Standoff)
                    {
                        bulletAttack.AddModdedDamageType(Bandit2Core.StandoffDamage);
                    }
                    else if (selectedPassive == Skills.DesperadoKillStack)
					{
						bulletAttack.damageType |= DamageType.GiveSkullOnKill;
                    }
                    else
                    {
                        bulletAttack.damageType |= DamageType.ResetCooldownsOnKill;
                    }

                    bulletAttack.damageType |= DamageType.BonusToLowHealth;
				}

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
				if (shotsFired >= baseShotCount)
                {
					this.outer.SetNextState(new ExitSidearm());
				}
				else
				{
					this.outer.SetNextState(new FireLightsOutScepter { shotsFired = this.shotsFired });
				}
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return (shotsFired >= baseShotCount) ? InterruptPriority.Any : InterruptPriority.Skill;
		}

		public override void LoadStats()
		{
			crosshairOverridePrefab = _crosshairOverridePrefab;
			baseDuration = shotsFired >= baseShotCount ? 1f : 0.15f;
		}

        public static GameObject _crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2CrosshairPrepRevolverFire.prefab").WaitForCompletion();

        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion();
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit2Pistol.prefab").WaitForCompletion();
        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/TracerBanditPistol.prefab").WaitForCompletion();

        public static float damageCoefficient = 9f;
		public static float force = 2000f;

		public static float minSpread = 0f;
		public static float maxSpread = 0f;

		public static string attackSoundString = "Play_bandit2_R_fire";
		public static float recoilAmplitude = 1f;
		public static float bulletRadius = 1f;

		public static int baseShotCount = 2;
		public int shotsFired = 0;
	}
}
