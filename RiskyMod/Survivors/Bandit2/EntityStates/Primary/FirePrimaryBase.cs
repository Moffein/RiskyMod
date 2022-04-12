using UnityEngine;
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

		public override void OnEnter()
		{
			buttonReleased = false;
			baseDuration = _baseDuration;
			if (BanditFireModes.enabled && BanditFireModes.currentfireMode == BanditFireModes.Bandit2FireMode.Spam) baseDuration = 0.1f;
			base.OnEnter();
			duration = baseDuration / this.attackSpeedStat;
			minDuration = baseMinDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireMainWeapon", "FireMainWeapon.playbackRate", baseAnimDuration/this.attackSpeedStat);
		}

		public override void ModifyBullet(BulletAttack bulletAttack)
		{
			base.ModifyBullet(bulletAttack);
			bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
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
				&& !(BanditFireModes.enabled && BanditFireModes.currentfireMode == BanditFireModes.Bandit2FireMode.Spam))
			{
				base.characterBody.SetSpreadBloom(0f, false);
			}
			base.OnExit();
		}
	}
}
