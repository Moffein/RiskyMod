using RoR2;
using UnityEngine;
using R2API;
using RoR2.Projectile;
using EntityStates;
using RoR2.Modding;

namespace EntityStates.RiskyMod.Croco
{
    public class FireDiseaseProjectileScepter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = GetAimRay();
            duration = baseDuration / attackSpeedStat;
            StartAimMode(duration + 2f, false);
            PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", duration);
            Util.PlaySound(attackString, gameObject);
            AddRecoil(-1f * recoilAmplitude, -1.5f * recoilAmplitude, -0.25f * recoilAmplitude, 0.25f * recoilAmplitude);
            characterBody.AddSpreadBloom(bloom);
            string muzzleName = "MouthMuzzle";
            if (effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, muzzleName, false);
            }
            if (isAuthority)
            {
                FireProjectileInfo fireProjectileInfo = default;
                fireProjectileInfo.projectilePrefab = projectilePrefab;
                fireProjectileInfo.position = aimRay.origin;
                fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
                fireProjectileInfo.owner = gameObject;
                fireProjectileInfo.damage = damageStat * damageCoefficient;
                fireProjectileInfo.force = force;
                fireProjectileInfo.crit = Util.CheckRoll(critStat, characterBody.master);

                CrocoDamageTypeController cdc = base.GetComponent<CrocoDamageTypeController>();
                if (cdc)
                {
                    fireProjectileInfo.damageTypeOverride = cdc.GetDamageType();
                }

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
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
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
