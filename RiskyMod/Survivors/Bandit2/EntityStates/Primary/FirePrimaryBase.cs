using UnityEngine;
using RoR2;

namespace EntityStates.RiskyMod.Bandit2.Primary
{
    public class FirePrimaryBase : GenericBulletBaseState
	{
		public static float _baseDuration = 0.3f;
		public static float baseMinDuration = 0f;
		private float minDuration;
		private bool buttonReleased;

		public override void OnEnter()
		{
			buttonReleased = false;
			baseDuration = _baseDuration;
			base.OnEnter();
			duration = baseDuration / this.attackSpeedStat;
			minDuration = baseMinDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireMainWeapon", "FireMainWeapon.playbackRate", this.duration);
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
			if (!buttonReleased && base.characterBody && base.skillLocator && base.skillLocator.primary.stock > 0)
			{
				base.characterBody.SetSpreadBloom(0f, false);
			}
			base.OnExit();
		}
	}
}
