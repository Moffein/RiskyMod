using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.Bandit2
{
    public class StealthMode : BaseState
	{

		public static float duration = 4f;
		public static float minDuration = 0.3f;
		public static string enterStealthSound = "Play_bandit2_shift_enter";
		public static string exitStealthSound = "Play_bandit2_shift_exit";
		public static float blastAttackRadius = 12f;
		public static float blastAttackDamageCoefficient = 2f;
		public static float blastAttackProcCoefficient = 1f;
		public static float blastAttackForce = 60f;
		public static GameObject smokeBombEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/Bandit2SmokeBomb");
		public static string smokeBombMuzzleString = "MuzzleSmokeBomb";
		public static float shortHopVelocity = 17f;

		private Animator animator;

		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();

			if (base.characterBody)
			{
				if (NetworkServer.active)
				{
					base.characterBody.AddBuff(RoR2Content.Buffs.Cloak);
					base.characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
				}
				base.characterBody.onSkillActivatedAuthority += this.OnSkillActivatedAuthority;
			}
			this.FireSmokebomb();
			Util.PlaySound(StealthMode.enterStealthSound, base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > StealthMode.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override void OnExit()
		{
			Util.PlaySound(StealthMode.exitStealthSound, base.gameObject);
			if (!this.outer.destroying)
			{
				this.FireSmokebomb();
			}
			if (base.characterBody)
			{
				base.characterBody.onSkillActivatedAuthority -= this.OnSkillActivatedAuthority;
				if (NetworkServer.active)
				{
					base.characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
					base.characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
				}
			}
			if (this.animator)
			{
				this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, StealthWeapon"), 0f);
			}
			base.OnExit();
		}

		private void OnSkillActivatedAuthority(GenericSkill skill)
		{
			if (skill.skillDef.isCombatSkill)
			{
				this.outer.SetNextStateToMain();
			}
		}

		private void FireSmokebomb()
		{
			if (base.isAuthority)
			{
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.radius = StealthMode.blastAttackRadius;
				blastAttack.procCoefficient = StealthMode.blastAttackProcCoefficient;
				blastAttack.position = base.transform.position;
				blastAttack.attacker = base.gameObject;
				blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
				blastAttack.baseDamage = base.characterBody.damage * StealthMode.blastAttackDamageCoefficient;
				blastAttack.falloffModel = BlastAttack.FalloffModel.None;
				blastAttack.damageType = DamageType.Stun1s;
				blastAttack.baseForce = StealthMode.blastAttackForce;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
				blastAttack.Fire();
			}

			if (StealthMode.smokeBombEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(StealthMode.smokeBombEffectPrefab, base.gameObject, StealthMode.smokeBombMuzzleString, false);
			}
			if (base.characterMotor)
			{
				base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, StealthMode.shortHopVelocity, base.characterMotor.velocity.z);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return base.fixedAge > minDuration ? InterruptPriority.Skill : InterruptPriority.PrioritySkill;
		}
	}
}
