using RoR2;
using UnityEngine;
using EntityStates;
using RiskyMod.Survivors.Captain;

namespace EntityStates.RiskyMod.Captain
{
    public class ChargeShotgun : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.minChargeDuration = baseMinChargeDuration/this.attackSpeedStat; // this.attackSpeedStat;
			this.chargeDuration = baseChargeDuration/this.attackSpeedStat; // this.attackSpeedStat;
			base.PlayCrossfade("Gesture, Override", "ChargeCaptainShotgun", "ChargeCaptainShotgun.playbackRate", this.chargeDuration, 0.1f);
			base.PlayCrossfade("Gesture, Additive", "ChargeCaptainShotgun", "ChargeCaptainShotgun.playbackRate", this.chargeDuration, 0.1f);
			this.muzzleTransform = base.FindModelChild(muzzleName);
			if (this.muzzleTransform)
			{
				this.chargeupVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(chargeupVfxPrefab, this.muzzleTransform);
				this.chargeupVfxGameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = this.chargeDuration;
			}
			this.enterSoundID = Util.PlaySound(enterSoundString, base.gameObject);
			Util.PlaySound(playChargeSoundString, base.gameObject);
			charge = 0f;
		}

		public override void OnExit()
		{
			if (this.chargeupVfxGameObject)
			{
				EntityState.Destroy(this.chargeupVfxGameObject);
				this.chargeupVfxGameObject = null;
			}
			if (this.holdChargeVfxGameObject)
			{
				EntityState.Destroy(this.holdChargeVfxGameObject);
				this.holdChargeVfxGameObject = null;
			}
			AkSoundEngine.StopPlayingID(this.enterSoundID);
			Util.PlaySound(stopChargeSoundString, base.gameObject);
			base.OnExit();
		}

		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(base.age / this.chargeDuration, true);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterBody.SetAimTimer(1f);
			charge = Mathf.Clamp01(base.fixedAge / this.chargeDuration);
			if (base.fixedAge >= this.chargeDuration)
			{
				if (this.chargeupVfxGameObject)
				{
					EntityState.Destroy(this.chargeupVfxGameObject);
					this.chargeupVfxGameObject = null;
				}
				if (!this.holdChargeVfxGameObject && this.muzzleTransform)
				{
					this.holdChargeVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(holdChargeVfxPrefab, this.muzzleTransform);
				}
			}
			if (base.isAuthority)
			{
				if (!this.released &&
					(!base.inputBank
					|| !base.inputBank.skill1.down
					|| (CaptainFireModes.enabled.Value &&
						(CaptainFireModes.currentfireMode == CaptainFireModes.CaptainFireMode.Auto
						|| (CaptainFireModes.currentfireMode == CaptainFireModes.CaptainFireMode.Charged && charge >= 1f))
						)
					)
				)
				{
					this.released = true;
				}
				if (this.released)
				{
					this.outer.SetNextState(new FireShotgun());
				}
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static float baseMinChargeDuration = 0.1f;
		public static float baseChargeDuration = 1.2f;
		public static string muzzleName = "MuzzleGun";
		public static GameObject chargeupVfxPrefab;
		public static GameObject holdChargeVfxPrefab;
		public static string enterSoundString = "Play_captain_m1_chargeStart";
		public static string playChargeSoundString = "Play_captain_m1_shotgun_charge_loop";
		public static string stopChargeSoundString = "Stop_captain_m1_shotgun_charge_loop";

		private float minChargeDuration;
		private float chargeDuration;
		private bool released;
		private GameObject chargeupVfxGameObject;
		private GameObject holdChargeVfxGameObject;
		private Transform muzzleTransform;
		private uint enterSoundID;
		private float charge;
	}
}
