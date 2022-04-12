using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyMod.MonoBehaviours
{
    public class ProjectileZapChainOnExplosion : MonoBehaviour
    {
        


        public float damageCoefficient;
        public float procCoefficient;
        public float range;
        public float targetsPerBounce;
        public float maxBounces;
        public DamageType damageType;
    }
}
