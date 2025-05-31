using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Bosses
{
    public class Gravekeeper
    {
        public static bool disableProjectileOnKill = true;
        public Gravekeeper()
        {
            if (disableProjectileOnKill)
            {
                RemoveGrovetenderWispOnKill();
            }
        }
        private void RemoveGrovetenderWispOnKill()
        {
            GameObject trackingWispObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Gravekeeper/GravekeeperTrackingFireball.prefab").WaitForCompletion();
            HealthComponent hc = trackingWispObject.GetComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
        }
    }
}
