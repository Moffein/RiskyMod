using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using RoR2.Projectile;

namespace EntityStates.RiskyMod.Commando.Scepter
{
    public class ThrowGrenadeScepter : ThrowGrenade
    {
        public static new GameObject _projectilePrefab;
        public static new float _damageCoefficient = 12f;
        public static new float _force = 3000f;

        public override void LoadStats()
        {
            base.LoadStats();
            force = _force;
            projectilePrefab = _projectilePrefab;
            damageCoefficient = _damageCoefficient;
        }
        public override void FireProjectile()
        {
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                aimRay = this.ModifyProjectileAimRay(aimRay);

                bool crit = base.RollCrit();

                ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * this.damageCoefficient, fuseTime, crit, DamageColorIndex.Default, null, -1f);

                for (int i = 0; i < 5; i++)
                {
                    Vector3 spreadDirection = Util.ApplySpread(aimRay.direction, 3f, 9f, 1f, 1f, 0f, this.projectilePitchBonus);
                    ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(spreadDirection), base.gameObject, 0.5f*this.damageStat * this.damageCoefficient, fuseTime, crit, DamageColorIndex.Default, null, -1f);
                }
            }
        }
    }
}
