using RoR2;

namespace EntityStates.RiskyMod.Bandit2
{
    public class ThrowSmokebomb : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Gesture, Additive", "ThrowSmokebomb", "ThrowSmokebomb.playbackRate", ThrowSmokebomb.duration);
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
