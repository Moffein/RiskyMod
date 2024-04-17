using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using RoR2.Projectile;

namespace EntityStates.RiskyModStates.Commando.Scepter
{
    public class ThrowGrenadeScepter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            if (base.GetModelAnimator())
            {
                base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", duration * 2f);
                base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", duration * 2f);
            }
            Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
            if (base.isAuthority)
            {
                bool isCrit = base.RollCrit();
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, 0f, pitchBonus);// base.projectilePitchBonus
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, 0f, isCrit, DamageColorIndex.Default, null, -1f);


                //Original scepter is half damage/knockback, throw 8 at once
                //This version is throw 1 + 4 at once, no damage penalty and half knockback on the extra grenades
                for (int i = 0; i < 4; i++)
                {
                    aimRay = base.GetAimRay();
                    aimRay.direction = Util.ApplySpread(aimRay.direction, 7f, 15f, 1f, 1f, 0f, pitchBonus);// base.projectilePitchBonus
                    ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.outer.gameObject, base.damageStat * damageCoefficient, force * 0.5f, isCrit, DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static float pitchBonus = -10f;
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 7f;
        public static float force = 1000f;
        public static float baseDuration = 0.5f;
        private float duration;
    }
}