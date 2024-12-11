﻿using UnityEngine;
using RoR2;
using RiskyMod.Survivors.Bandit2;

namespace EntityStates.RiskyMod.Bandit2.Primary
{
    public class FirePrimaryBase : GenericBulletBaseState
	{
		public static float _baseDuration = 0.3f;
		public static float baseMinDuration = 0f;

		public static float baseAnimDuration = 1f;


		private float minDuration;
		private bool buttonReleased;

		private bool fireSelectEnabled;
		private bool isSpam;

		public override void OnEnter()
		{
			buttonReleased = false;
			baseDuration = _baseDuration;

			fireSelectEnabled = RiskyTweaks.Tweaks.Survivors.Bandit2.PrimaryAutoFire.Instance.Enabled.Value && RiskyTweaks.Tweaks.Survivors.Bandit2.PrimaryAutoFire.FireMode.Enabled.Value;
			isSpam = RiskyTweaks.Tweaks.Survivors.Bandit2.PrimaryAutoFire.FireMode.currentfireMode == RiskyTweaks.Tweaks.Survivors.Bandit2.PrimaryAutoFire.FireMode.Bandit2FireMode.Spam;

            if (fireSelectEnabled && isSpam) baseDuration = 0.12f;
			base.OnEnter();
			duration = baseDuration / this.attackSpeedStat;
			minDuration = baseMinDuration / this.attackSpeedStat;

			bool playAnim = true;
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				int layerIndex = modelAnimator.GetLayerIndex("Gesture, Additive");
				if (layerIndex >= 0)
				{
					AnimatorStateInfo animStateInfo = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex);
					if (animStateInfo.IsName("SlashBlade"))
					{
						playAnim = false;
					}
				}
			}
			if (playAnim) base.PlayAnimation("Gesture, Additive", "FireMainWeapon", "FireMainWeapon.playbackRate", baseAnimDuration/this.attackSpeedStat);
		}

		public override void ModifyBullet(BulletAttack bulletAttack)
		{
			base.ModifyBullet(bulletAttack);
			bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
			bulletAttack.damageType.damageSource = DamageSource.Primary;
		}

        public override void FixedUpdate()
        {
            base.FixedUpdate();
			if (!buttonReleased && !(base.inputBank && base.inputBank.skill1.down))
            {
				buttonReleased = true;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (base.fixedAge <= minDuration || !buttonReleased)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Any;
		}

        public override void OnExit()
		{
			if (!buttonReleased && base.characterBody && base.skillLocator && base.skillLocator.primary.stock > 0
				&& !(fireSelectEnabled && isSpam))
			{
				base.characterBody.SetSpreadBloom(0f, false);
			}
			base.OnExit();
		}
	}
}
