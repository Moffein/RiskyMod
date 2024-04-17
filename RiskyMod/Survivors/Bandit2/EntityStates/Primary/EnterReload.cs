using RoR2;
using UnityEngine;

namespace EntityStates.RiskyModStates.Bandit2.Primary
{
    public class EnterReload : BaseState
	{
		//public static string enterSoundString;
		public static float baseDuration = 0.3f;

		private float duration
		{
			get
			{
				return EnterReload.baseDuration / this.attackSpeedStat;
			}
		}

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
			if (playAnim) base.PlayCrossfade("Gesture, Additive", "EnterReload", "Reload.playbackRate", this.duration, 0.1f);
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
