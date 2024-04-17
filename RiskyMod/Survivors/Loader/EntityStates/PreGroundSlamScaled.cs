using RoR2;

namespace EntityStates.RiskyModStates.Loader
{
	public class PreGroundSlamScaled : BaseCharacterMain
	{
		// Token: 0x06003FE3 RID: 16355 RVA: 0x000FD4B4 File Offset: 0x000FB6B4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = PreGroundSlamScaled.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Body", "PreGroundSlam", "GroundSlam.playbackRate", this.duration);
			Util.PlaySound(PreGroundSlamScaled.enterSoundString, base.gameObject);
			base.characterMotor.Motor.ForceUnground();
			base.characterMotor.disableAirControlUntilCollision = false;
			base.characterMotor.velocity.y = PreGroundSlamScaled.upwardVelocity;
		}

		// Token: 0x06003FE4 RID: 16356 RVA: 0x000FD536 File Offset: 0x000FB736
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterMotor.moveDirection = base.inputBank.moveVector;
			if (base.fixedAge > this.duration)
			{
				this.outer.SetNextState(new GroundSlamScaled());
			}
		}

		public static float baseDuration = 0.4f;
		public static string enterSoundString = "Play_loader_R_variant_activate";
		public static float upwardVelocity = 12f;
		private float duration;
	}
}
