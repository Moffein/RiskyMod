using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using RoR2.Projectile;

namespace EntityStates.RiskyMod.Commando.Scepter
{
    public class ThrowGrenadeScepter : ThrowGrenade
    {
        public static new GameObject _projectilePrefab;
        public static new float _damageCoefficient = 20f;
        public static new float _force = 3000f;

        public override void LoadStats()
        {
            base.LoadStats();
            force = _force;
            projectilePrefab = _projectilePrefab;
            damageCoefficient = _damageCoefficient;
        }
    }
}
