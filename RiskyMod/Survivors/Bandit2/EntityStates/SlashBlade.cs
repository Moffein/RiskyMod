using RoR2;
using UnityEngine;
using RiskyMod.Survivors.Bandit2;
using R2API;

namespace EntityStates.RiskyMod.Bandit2
{
    public class SlashBlade : BasicMeleeAttack
    {
		private float minimumDuration
		{
			get
			{
				return SlashBlade.minimumBaseDuration / this.attackSpeedStat;
			}
		}

		public override void OnEnter()
		{
			LoadStats();
			base.OnEnter();
			base.PlayAnimation("Gesture, Additive", "SlashBlade", "SlashBlade.playbackRate", this.duration);
			this.bladeMeshObject = base.FindModelChild("BladeMesh").gameObject;
			if (this.bladeMeshObject)
			{
				this.bladeMeshObject.SetActive(true);
			}
			base.characterMotor.ApplyForce(base.inputBank.moveVector * SlashBlade.selfForceStrength, true, false);
			if (base.characterMotor)
			{
				base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, Mathf.Max(base.characterMotor.velocity.y, SlashBlade.shortHopVelocity), base.characterMotor.velocity.z);
			}
		}

		public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
			base.AuthorityModifyOverlapAttack(overlapAttack);
			overlapAttack.damageType = DamageType.SuperBleedOnCrit;
			DamageAPI.AddModdedDamageType(overlapAttack, Bandit2Core.AlwaysBackstab);
		}

		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(SlashBlade.bloomCurve.Evaluate(base.age / this.duration), false);
		}

		public override void OnExit()
		{
			if (this.bladeMeshObject)
			{
				this.bladeMeshObject.SetActive(false);
			}
			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (!base.inputBank)
			{
				return InterruptPriority.Skill;
			}
			if (base.fixedAge <= this.minimumDuration)
			{
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Skill;
		}

		private void LoadStats()
		{
			baseDuration = 0.6f;
			damageCoefficient = 3.6f;
			hitBoxGroupName = "SlashBlade";
			hitEffectPrefab = _hitEffectPrefab;
			procCoefficient = 1f;
			pushAwayForce = 0;
			forceVector = Vector3.zero;
			hitPauseDuration = 0.05f;
			swingEffectPrefab = _swingEffectPrefab;
			swingEffectMuzzleString = "MuzzleSlashBlade";
			mecanimHitboxActiveParameter = "SlashBlade.hitBoxActive";
			shorthopVelocityFromHit = 6f;
			impactSound = _impactSound;
			forceForwardVelocity = true;
			beginStateSoundString = "Play_bandit2_m2_slash";
			forwardVelocityCurve = _forwardVelocityCurve;
			scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
			ignoreAttackSpeed = false;
		}

		public static float shortHopVelocity = 0f;
		public static float selfForceStrength = 0f;
		public static float minimumBaseDuration = 0.3f;

		public static NetworkSoundEventDef _impactSound = Resources.Load<NetworkSoundEventDef>("networksoundeventdefs/nseBandit2ShivHit");
		public static GameObject _hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");
		public static GameObject _swingEffectPrefab;
		public static AnimationCurve bloomCurve;
		public static AnimationCurve _forwardVelocityCurve;

		private GameObject bladeMeshObject;
	}
}
