using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Commando
{
    //Copypasted FireFMJ
    public class FireLightningRound : GenericProjectileBaseState
    {
        public override void OnEnter()
        {
            effectPrefab = lightningMuzzleflashEffect;
            projectilePrefab = lightningProjectilePrefab;
            force = 2000f;
            damageCoefficient = 4.5f;
            baseDuration = 0.5f;
            minSpread = 0f;
            maxSpread = 0f;
            recoilAmplitude = 1.5f;
            attackSoundString = "Play_commando_M2";
            projectilePitchBonus = 0f;
            baseDelayBeforeFiringProjectile = 0f;
            recoilAmplitude = 1.5f;
            bloom = -1f;
            targetMuzzle = "MuzzleCenter";
            base.OnEnter();
        }

        public override void PlayAnimation(float duration)
        {
            base.PlayAnimation(duration);
            if (base.GetModelAnimator())
            {
                base.PlayAnimation("Gesture, Additive", FireFMJStateHash, FireFMJParamHash, duration);
                base.PlayAnimation("Gesture, Override", FireFMJStateHash, FireFMJParamHash, duration);
            }
        }

        public override Ray ModifyProjectileAimRay(Ray aimRay)
        {
            TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref aimRay, this.projectilePrefab, base.gameObject, 1f);
            return aimRay;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject lightningMuzzleflashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
        public static GameObject lightningProjectilePrefab;
        private static int FireFMJStateHash = Animator.StringToHash("FireFMJ");
        private static int FireFMJParamHash = Animator.StringToHash("FireFMJ.playbackRate");
    }
}
