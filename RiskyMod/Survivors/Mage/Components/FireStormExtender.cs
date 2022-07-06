using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace RiskyMod.Survivors.Mage.Components
{
    public class FireStormExtender : MonoBehaviour
    {
        public static int maxExtensions = 1;
        public static float extensionDelay = 3f;
        public static GameObject projectilePrefab;

        public float stopwatch;
        public int extensionCount;

        public void Awake()
        {
            stopwatch = 0f;
            extensionCount = 0;
        }
        
        public void FixedUpdate()
        {
            if (NetworkServer.active && extensionCount < maxExtensions)
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= extensionDelay)
                {
                    stopwatch -= extensionDelay;
                    Extend();
                }
            }
        }

        private void Extend()
        {
            extensionCount++;

            ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
            ProjectileController pc = base.GetComponent<ProjectileController>();


            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
            {
                damage = pd ? pd.damage : 0f,
                crit = pd.crit,
                damageColorIndex = DamageColorIndex.Default,
                position = base.transform.position,
                procChainMask = default,
                force = 0f,
                owner = pc ? pc.owner : null,
                projectilePrefab = FireStormExtender.projectilePrefab,
                rotation = Quaternion.identity,
                speedOverride = 0f,
                target = null
            });
        }
    }
}
