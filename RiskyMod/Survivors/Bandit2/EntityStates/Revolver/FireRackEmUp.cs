using R2API;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using RoR2.Skills;
using UnityEngine;
namespace EntityStates.RiskyMod.Bandit2.Revolver
{
	public class FireRackEmUp : BaseSidearmState
	{
		public virtual int GetBulletCount()
        {
			return baseBulletCount;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			modelAnimator = base.GetModelAnimator();
            scaledRecoilAmplitude = recoilAmplitude / this.attackSpeedStat;
			FireBullet();
        }

		private void FireBullet()
        {
            shotsFired++;
            Util.PlaySound(attackSoundString, base.gameObject);
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            string muzzleName = "MuzzlePistol";
            bool playAnim = true;
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
                bulletAttack.maxDistance = 200f;

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
                DamageAPI.AddModdedDamageType(bulletAttack, Bandit2Core.RackEmUpDamage);

                bulletAttack.Fire();
            }
            if (playAnim) base.PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", shotsFired >= baseBulletCount ? 1f : duration);
            if (effectPrefab) EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, muzzleName, false);
            base.AddRecoil(-3f * scaledRecoilAmplitude, -4f * scaledRecoilAmplitude, -0.5f * scaledRecoilAmplitude, 0.5f * scaledRecoilAmplitude);
        }

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

            fireStopwatch += Time.fixedDeltaTime;
            if(fireStopwatch >= duration)
            {
                fireStopwatch -= duration;
                FireBullet();
            }

			if (base.isAuthority && shotsFired >= GetBulletCount())
			{
                this.outer.SetNextState(new ExitSidearm());
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
            baseDuration = 0.13f;
        }

		public static GameObject _crosshairOverridePrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/crosshair/Bandit2CrosshairPrepRevolverFire");

		public static GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashBandit2");
		public static GameObject hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkBandit2Pistol");
		public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/TracerBanditPistol");

		public static float damageCoefficient = 1f;
		public static float bonusDamageCoefficient = 0.2f;
		public static float force = 300f;

		public static float minSpread = 0f;
		public static float maxSpread = 2.5f;

		public static string attackSoundString = "Play_bandit2_R_fire";
		public static float recoilAmplitude = 1.5f;
		public static float bulletRadius = 0.5f;

		public static int baseBulletCount = 6;
		public int shotsFired = 0;

        private float scaledRecoilAmplitude;
        private float fireStopwatch = 0f;
		private Animator modelAnimator;
	}
}
