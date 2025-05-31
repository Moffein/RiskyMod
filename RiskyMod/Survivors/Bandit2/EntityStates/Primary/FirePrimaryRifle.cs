using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Bandit2.Primary
{
    public class FirePrimaryRifle : FirePrimaryBase
    {
        public static GameObject _muzzleFlashPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion();
        public static GameObject _tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/TracerBandit2Rifle.prefab").WaitForCompletion();
        public static GameObject _hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion();

        private void LoadStats()
        {
            damageCoefficient = 3.6f;
            procCoefficient = 1f;
            bulletCount = 1;
            maxDistance = 1000f;
            bulletRadius = 0.4f;
            useSmartCollision = true;
            force = 1000f;
            minSpread = 0f;
            maxSpread = 0f;
            spreadPitchScale = 0.5f;
            spreadYawScale = 1.3f;
            spreadBloomValue = 0.4f;
            recoilAmplitudeY = 1f;
            recoilAmplitudeX = 0.5f;
            muzzleName = "MuzzleShotgun";
            fireSoundString = "Play_bandit2_m1_rifle";
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
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            bulletAttack.stopperMask = LayerIndex.world.mask;
        }
    }
}
