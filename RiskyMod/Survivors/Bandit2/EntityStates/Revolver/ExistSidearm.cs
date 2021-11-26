namespace EntityStates.RiskyMod.Bandit2.Revolver
{
	public class ExitSidearm : BaseSidearmState
	{
		public override string exitAnimationStateName
		{
			get
			{
				return "SideToMain";
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}
	}
}
