using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.Bandit2.Primary
{
    public class FirePrimaryShotgun : FirePrimaryBase
    {
        public static GameObject _muzzleFlashPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashBandit2");
        public static GameObject _tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/TracerBandit2Shotgun");
        public static GameObject _hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkBandit");

        public float minFixedSpreadYaw = 1f;
        public float maxFixedSpreadYaw = 1f;

        private void LoadStats()
        {
            damageCoefficient = 0.9f;
            procCoefficient = 0.7f;
            bulletCount = 5;
            maxDistance = 1000f;
            bulletRadius = 0.3f;
            useSmartCollision = true;
            force = 320f;
            minSpread = 0f;
            maxSpread = 0f;
            spreadPitchScale = 0f;
            spreadYawScale = 0f;
            spreadBloomValue = 0.6f;
            recoilAmplitudeY = 1f;
            recoilAmplitudeX = 0.5f;
            muzzleName = "MuzzleShotgun";
            fireSoundString = "Play_bandit2_m1_shotgun";
            muzzleFlashPrefab = _muzzleFlashPrefab;
            tracerEffectPrefab = _tracerEffectPrefab;
            hitEffectPrefab = _hitEffectPrefab;
        }

        public override void OnEnter()
        {
            LoadStats();
            base.OnEnter();
        }

        public override void ModifyBullet(BulletAttack bulletAttack)
        {
            base.ModifyBullet(bulletAttack);
            bulletAttack.bulletCount = 1;
            //bulletAttack.stopperMask = LayerIndex.world.mask;
        }
        public override void FireBullet(Ray aimRay)
        {
            base.StartAimMode(aimRay, 3f, false);
            this.DoFireEffects();
            this.PlayFireAnimation();
            base.AddRecoil(-1f * this.recoilAmplitudeY, -1.5f * this.recoilAmplitudeY, -1f * this.recoilAmplitudeX, 1f * this.recoilAmplitudeX);
            if (base.isAuthority)
            {
                Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                Vector3 axis = Vector3.Cross(aimRay.direction, rhs);
                float currentSpread = 0f;
                if (base.characterBody)
                {
                    currentSpread = base.characterBody.spreadBloomAngle;
                }
                float angle = 0f;
                float num2 = 0f;
                if (this.bulletCount > 1)
                {
                    num2 = UnityEngine.Random.Range(this.minFixedSpreadYaw + currentSpread, this.maxFixedSpreadYaw + currentSpread) * 2f;
                    angle = num2 / (float)(this.bulletCount - 1);
                }
                Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                Ray aimRay2 = new Ray(aimRay.origin, direction);
                for (int i = 0; i < this.bulletCount; i++)
                {
                    BulletAttack bulletAttack = base.GenerateBulletAttack(aimRay2);
                    this.ModifyBullet(bulletAttack);
                    bulletAttack.Fire();
                    aimRay2.direction = rotation * aimRay2.direction;
                }
            }
        }
    }
}
