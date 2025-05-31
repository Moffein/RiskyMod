using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RiskyMod.Bandit2
{
    public class Reload : BaseState
	{
		public static float enterSoundPitch = 1f;
		public static float exitSoundPitch = 1f;
		public static string enterSoundString = "Play_bandit2_m1_reload_bullet";
		public static string exitSoundString = "Play_bandit2_m1_reload_finish";
		public static GameObject reloadEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Reload.prefab").WaitForCompletion();
		public static string reloadEffectMuzzleString = "PalmL";
		public static float baseDuration = 0.3f;

		private bool hasGivenStock;

		private float duration
		{
			get
			{
				return Reload.baseDuration / this.attackSpeedStat;
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
			if (playAnim) base.PlayAnimation("Gesture, Additive", (base.characterBody.isSprinting && base.characterMotor && base.characterMotor.isGrounded) ? "ReloadSimple" : "Reload", "Reload.playbackRate", this.duration);
			Util.PlayAttackSpeedSound(Reload.enterSoundString, base.gameObject, Reload.enterSoundPitch);
			EffectManager.SimpleMuzzleFlash(Reload.reloadEffectPrefab, base.gameObject, Reload.reloadEffectMuzzleString, false);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration / 2f)
			{
				this.GiveStock();
			}
			if (!base.isAuthority || base.fixedAge < this.duration)
			{
				return;
			}
			if (base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
			{
				this.outer.SetNextState(new Reload());
				return;
			}
			Util.PlayAttackSpeedSound(Reload.exitSoundString, base.gameObject, Reload.exitSoundPitch);
			this.outer.SetNextStateToMain();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		private void GiveStock()
		{
			if (this.hasGivenStock)
			{
				return;
			}
			if (base.isAuthority && base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
			{
				base.skillLocator.primary.AddOneStock();
			}
			this.hasGivenStock = true;
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}
