using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyMod.Survivors.Commando
{
    public class GrenadeImpactComponent : MonoBehaviour
    {
        private ProjectileImpactExplosion pie;

        public void Awake()
        {
            pie = base.GetComponent<ProjectileImpactExplosion>();
            if (!pie) Destroy(this);
        }

        public void FixedUpdate()
        {
            if (pie.stopwatchAfterImpact > 0f)
            {
                pie.destroyOnEnemy = false;
                Destroy(this);
                return;
            }
        }
    }
}
