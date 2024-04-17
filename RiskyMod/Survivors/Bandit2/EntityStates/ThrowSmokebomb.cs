using RoR2;
using UnityEngine;

namespace EntityStates.RiskyMod.Bandit2
{
    public class ThrowSmokebomb : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();

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
			if (playAnim) base.PlayAnimation("Gesture, Additive", "ThrowSmokebomb", "ThrowSmokebomb.playbackRate", ThrowSmokebomb.duration);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > ThrowSmokebomb.duration)
			{
				this.outer.SetNextState(new StealthMode());
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public static float duration = 0.1f;
	}
}
