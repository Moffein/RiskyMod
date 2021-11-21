using RoR2;
using UnityEngine;

namespace EntityStates.RiskyMod.Bandit2
{
    public class EnterReload : BaseState
	{
		//public static string enterSoundString;
		public static float baseDuration = 0.3f;

		private float duration
		{
			get
			{
				//In this mod, all reload entries have a minimum entry duration to prevent machinegunning at max attack speed, since that defeats the purpose of having a reload.
				//The actual part where shots are loaded is affected by attack speed. This is just so that you need to let go of the trigger to benefit from it.
				return EnterReload.baseDuration; // this.attackSpeedStat;
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Gesture, Additive", "EnterReload", "Reload.playbackRate", this.duration, 0.1f);
			//Util.PlaySound(EnterReload.enterSoundString, base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge > this.duration)
			{
				this.outer.SetNextState(new Reload());
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}
