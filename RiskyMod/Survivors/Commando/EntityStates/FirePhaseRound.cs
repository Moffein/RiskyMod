using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.RiskyMod.Commando
{
	public class FirePhaseRound : GenericProjectileBaseState
	{
		public static GameObject _effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashFMJ");
		public static GameObject _projectilePrefab;

		private void LoadStats()
        {
			base.effectPrefab = _effectPrefab;
			base.projectilePrefab = _projectilePrefab;
			base.damageCoefficient = 4.8f;
			base.force = 2000f;
			base.minSpread = 0f;
			base.maxSpread = 0f;
			base.baseDuration = 0.5f;
			base.recoilAmplitude = 1.5f;
			base.attackSoundString = "Play_commando_m2";
			base.projectilePitchBonus = 0f;
			base.baseDelayBeforeFiringProjectile = 0f;
			base.targetMuzzle = "MuzzleCenter";
			base.bloom = -1f;
		}

        public override void OnEnter()
        {
			LoadStats();
            base.OnEnter();
        }

        public override void PlayAnimation(float duration)
		{
			base.PlayAnimation(duration);
			if (base.GetModelAnimator())
			{
				base.PlayAnimation("Gesture, Additive", "FireFMJ", "FireFMJ.playbackRate", duration);
				base.PlayAnimation("Gesture, Override", "FireFMJ", "FireFMJ.playbackRate", duration);
			}
		}

        public override InterruptPriority GetMinimumInterruptPriority()
        {
			return InterruptPriority.PrioritySkill;
        }
    }
}
