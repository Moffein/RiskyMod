using RoR2;
using UnityEngine;

namespace RiskyMod.Enemies.Bosses
{
    public class Gravekeeper
    {
        public static bool enabled = true;
        public Gravekeeper()
        {
            if (!enabled) return;
            GameObject trackingWispObject = Resources.Load<GameObject>("prefabs/projectiles/GravekeeperTrackingFireball");
            HealthComponent hc = trackingWispObject.GetComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
        }
    }
}
