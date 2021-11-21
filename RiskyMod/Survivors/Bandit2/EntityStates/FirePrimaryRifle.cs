using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.Bandit2
{
    public class FirePrimaryRifle : FirePrimaryBase
    {
        public static float _damageCoefficient = 3.3f;
        public static float _procCoefficient = 1f;
        public static int _bulletCount = 1;
        public static float _maxDistance = 1000f;
        public static float _bulletRadius = 0.4f;
        public static bool _useSmartCollision = true;
        public static float _force = 1000f;
        public static float _minSpread = 0f;
        public static float _maxSpread = 0f;
        public static float _spreadPitchScale = 0.5f;
        public static float _spreadYawScale = 1.3f;
        public static float _spreadBloomValue = 0.6f;
        public static float _recoilAmplitudeY = 1f;
        public static float _recoilAmplitudeX = 0.5f;
        public static string _muzzleName = "MuzzleShotgun";
        public static string _fireSoundString = "Play_bandit2_m1_rifle";
        public static GameObject _muzzleFlashPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashBandit2");
        public static GameObject _tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/TracerBandit2Rifle");
        public static GameObject _hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/HitsparkBandit");

        private void LoadStatics()
        {
            damageCoefficient = _damageCoefficient;
            procCoefficient = _procCoefficient;
            bulletCount = _bulletCount;
            maxDistance = _maxDistance;
            bulletRadius = _bulletRadius;
            useSmartCollision = _useSmartCollision;
            force = _force;
            minSpread = _minSpread;
            maxSpread = _maxSpread;
            spreadPitchScale = _spreadPitchScale;
            spreadYawScale = _spreadYawScale;
            spreadBloomValue = _spreadBloomValue;
            recoilAmplitudeY = _recoilAmplitudeY;
            recoilAmplitudeX = _recoilAmplitudeX;
            muzzleName = _muzzleName;
            fireSoundString = _fireSoundString;
            muzzleFlashPrefab = _muzzleFlashPrefab;
            tracerEffectPrefab = _tracerEffectPrefab;
            hitEffectPrefab = _hitEffectPrefab;
        }

        public override void OnEnter()
        {
            LoadStatics();
            base.OnEnter();
        }
        public override void ModifyBullet(BulletAttack bulletAttack)
        {
            base.ModifyBullet(bulletAttack);
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            bulletAttack.stopperMask = LayerIndex.world.mask;
        }
    }
}
