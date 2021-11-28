using RoR2;
using UnityEngine;
using RiskyMod.Survivors.Bandit2;

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

		// Token: 0x06004767 RID: 18279 RVA: 0x001214DC File Offset: 0x0011F6DC
		public override void OnEnter()
		{
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

		public static float shortHopVelocity;

		public static float selfForceStrength;

		public static float minimumBaseDuration;

		public static AnimationCurve bloomCurve;

		private GameObject bladeMeshObject;
	}
}
