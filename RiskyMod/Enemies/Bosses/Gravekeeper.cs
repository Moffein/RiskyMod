using RoR2;
using UnityEngine;

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
            GameObject trackingWispObject = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/GravekeeperTrackingFireball");
            HealthComponent hc = trackingWispObject.GetComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
        }
    }
}
