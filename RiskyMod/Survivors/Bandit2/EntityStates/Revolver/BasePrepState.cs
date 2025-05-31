﻿using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Bandit2.Revolver
{
	public abstract class BasePrepState : BaseSidearmState
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
			if (playAnim) base.PlayAnimation("Gesture, Additive", "MainToSide", "MainToSide.playbackRate", this.duration);
			Util.PlaySound(enterSoundString, base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge > this.duration &&  base.inputBank && !base.inputBank.skill4.down)
			{
				this.outer.SetNextState(this.GetNextState());
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}

        public override void LoadStats()
        {
			crosshairOverridePrefab = _crosshairOverridePrefab;
			baseDuration = 0.7f;
		}

        protected abstract EntityState GetNextState();

		public static string enterSoundString = "Play_bandit2_R_load";
		public static GameObject _crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2CrosshairPrepRevolver.prefab").WaitForCompletion();


		private Animator animator;
	}
}
