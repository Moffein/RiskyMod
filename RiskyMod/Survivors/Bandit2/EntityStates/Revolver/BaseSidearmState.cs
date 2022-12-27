using RoR2.UI;
using UnityEngine;

namespace EntityStates.RiskyMod.Bandit2.Revolver
{
	public class BaseSidearmState : BaseState
	{
		public virtual string exitAnimationStateName
		{
			get
			{
				return "BufferEmpty";
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			LoadStats();
			this.animator = base.GetModelAnimator();
			this.duration = this.baseDuration / this.attackSpeedStat;
			if (this.animator)
			{
				this.bodySideWeaponLayerIndex = this.animator.GetLayerIndex("Body, SideWeapon");
				this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 1f);
			}
			if (this.crosshairOverridePrefab)
			{
				this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, this.crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
			}
			base.characterBody.SetAimTimer(3f);
		}

		public override void OnExit()
		{
			if (this.animator)
			{
				this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 0f);
			}

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
			if (playAnim) base.PlayAnimation("Gesture, Additive", this.exitAnimationStateName);
			if (this.crosshairOverrideRequest != null)
            {
				this.crosshairOverrideRequest.Dispose();
			}
			Transform transform = base.FindModelChild("SpinningPistolFX");
			if (transform)
			{
				transform.gameObject.SetActive(false);
			}
			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public virtual void LoadStats() { }

		[SerializeField]
		public float baseDuration;
		[SerializeField]
		public GameObject crosshairOverridePrefab;
		protected float duration;
		private Animator animator;
		private int bodySideWeaponLayerIndex;
		private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
	}
}
