using EntityStates.Captain.Weapon;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Captain
{
    public class CallSupplyDropSkillRestock : CallSupplyDropBase
    {
        public override void OnEnter()
        {
            supplyDropPrefab = CallSupplyDropSkillRestock._supplyDropPrefab;
            muzzleflashEffect = _muzzleflashEffect;
            base.OnEnter();
        }

        public static GameObject _muzzleflashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/MuzzleflashSupplyDrop, Healing.prefab").WaitForCompletion();
        public static GameObject _supplyDropPrefab;
    }
}
