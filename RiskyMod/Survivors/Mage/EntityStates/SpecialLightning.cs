using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.Mage
{
	public class SpecialLightning : BaseState
	{
		public virtual void LoadStats()
		{
			loadBaseTickCount = SpecialLightning.baseTickCount;
			loadBaseTickFrequency = SpecialLightning.baseTickFrequency;
			loadMaxDistance = SpecialLightning.maxDistance;
		}

		public virtual void ModifyAttack(LightningOrb lo) { }

		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;

			LoadStats();

			this.entryDuration = SpecialLightning.baseEntryDuration / this.attackSpeedStat;

			this.tickFrequency = loadBaseTickFrequency * this.attackSpeedStat;
			//totalTickCount = Mathf.FloorToInt(loadBaseTickCount * this.attackSpeedStat);

			this.flamethrowerDuration = SpecialLightning.baseAttackDuration;

			//effectResetStopwatch = 0f;

			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.entryDuration + this.flamethrowerDuration + 1f);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
				this.leftMuzzleTransform = this.childLocator.FindChild("MuzzleLeft");
				this.rightMuzzleTransform = this.childLocator.FindChild("MuzzleRight");
			}

			if (base.isAuthority && base.characterBody)
			{
				this.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			}
			base.PlayAnimation("Gesture, Additive", "PrepFlamethrower", "Flamethrower.playbackRate", this.entryDuration);
		}

		public override void OnExit()
		{
			Util.PlaySound(SpecialLightning.endAttackSoundString, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "ExitFlamethrower", 0.1f);
			if (this.leftFlamethrowerTransform)
			{
				EntityState.Destroy(this.leftFlamethrowerTransform.gameObject);
			}
			if (this.rightFlamethrowerTransform)
			{
				EntityState.Destroy(this.rightFlamethrowerTransform.gameObject);
			}
			base.OnExit();
		}

		private void FireGauntlet(string muzzleString)
		{
			//totalTickCount--;
			Util.PlaySound(SpecialLightning.attackSoundString, base.gameObject); ;
			Ray aimRay = base.GetAimRay();
			if (NetworkServer.active)
			{
				List<HealthComponent> bouncedObjects = new List<HealthComponent>();
				LightningOrb lo = new LightningOrb
				{
					bouncedObjects = bouncedObjects,
					attacker = base.gameObject,
					inflictor = base.gameObject,
					damageValue = base.damageStat * SpecialLightning.tickDamageCoefficient,
					procCoefficient = SpecialLightning.procCoefficientPerTick,
					teamIndex = base.GetTeam(),
					isCrit = this.isCrit,
					procChainMask = default,
					lightningType = LightningOrb.LightningType.MageLightning,
					damageColorIndex = DamageColorIndex.Default,
					bouncesRemaining = 2,
					targetsToFindPerBounce = 3,
					range = SpecialLightning.bounceDistance,
					origin = aimRay.origin,
					damageType = DamageType.SlowOnHit,
					speed = 120f
				};
				ModifyAttack(lo);

				BullseyeSearch search = new BullseyeSearch();
				search.teamMaskFilter = TeamMask.allButNeutral;
				search.teamMaskFilter.RemoveTeam(characterBody.teamComponent.teamIndex);
				search.filterByLoS = false;
				search.searchOrigin = aimRay.origin;
				search.sortMode = BullseyeSearch.SortMode.Angle;
				search.maxDistanceFilter = SpecialLightning.maxDistance;
				search.maxAngleFilter = 60f;
				search.searchDirection = aimRay.direction;
				search.RefreshCandidates();

				HurtBox target = search.GetResults().FirstOrDefault();
				if (target)
                {
					lo.target = target;
					OrbManager.instance.AddOrb(lo);
					lo.bouncedObjects.Add(target.healthComponent);
				}

				if (base.characterMotor)
				{
					base.characterMotor.ApplyForce(aimRay.direction * -SpecialLightning.recoilForce, false, false);
				}
			}
		}

		public void InitFlamethrowerEffect()
		{
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild("MuzzleLeft");
				Transform transform2 = this.childLocator.FindChild("MuzzleRight");
				if (transform)
				{
					this.leftFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(SpecialLightning.flamethrowerEffectPrefab, transform).transform;
				}
				if (transform2)
				{
					this.rightFlamethrowerTransform = UnityEngine.Object.Instantiate<GameObject>(SpecialLightning.flamethrowerEffectPrefab, transform2).transform;
				}
				if (this.leftFlamethrowerTransform)
				{
					this.leftFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
				}
				if (this.rightFlamethrowerTransform)
				{
					this.rightFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = this.flamethrowerDuration;
				}
			}
		}

		private void ResetFlamethrowerEffect()
		{
			if (this.leftFlamethrowerTransform)
			{
				EntityState.Destroy(this.leftFlamethrowerTransform.gameObject);
				this.leftFlamethrowerTransform = null;
			}
			if (this.rightFlamethrowerTransform)
			{
				EntityState.Destroy(this.rightFlamethrowerTransform.gameObject);
				this.rightFlamethrowerTransform = null;
			}
			InitFlamethrowerEffect();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;

			if (this.stopwatch >= this.entryDuration && !this.hasBegunAttack)
			{
				this.hasBegunAttack = true;
				Util.PlaySound(SpecialLightning.startAttackSoundString, base.gameObject);
				base.PlayAnimation("Gesture, Additive", "Flamethrower", "Flamethrower.playbackRate", this.flamethrowerDuration);
				InitFlamethrowerEffect();
				this.FireGauntlet("MuzzleCenter");
			}
			if (this.hasBegunAttack)
			{
				//effectResetStopwatch += Time.fixedDeltaTime;
				this.flamethrowerStopwatch += Time.deltaTime;
				if (this.flamethrowerStopwatch > 1f / tickFrequency)
				{
					this.flamethrowerStopwatch -= 1f / tickFrequency;
					this.FireGauntlet("MuzzleCenter");
				}
				this.UpdateFlamethrowerEffect();
			}

			/*if (effectResetStopwatch > SpecialLightning.effectResetDuration)
			{
				effectResetStopwatch = 0f;
				if (totalTickCount * Mathf.Max(1f / tickFrequency, Time.fixedDeltaTime) > 0.6f)
				{
					ResetFlamethrowerEffect();
				}
			}*/

			if (base.fixedAge > (SpecialLightning.baseEntryDuration + SpecialLightning.baseAttackDuration) && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		private void UpdateFlamethrowerEffect()
		{
			Ray aimRay = base.GetAimRay();
			Vector3 direction = aimRay.direction;
			Vector3 direction2 = aimRay.direction;
			if (this.leftFlamethrowerTransform)
			{
				this.leftFlamethrowerTransform.forward = direction;
			}
			if (this.rightFlamethrowerTransform)
			{
				this.rightFlamethrowerTransform.forward = direction2;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static GameObject flamethrowerEffectPrefab;
		public static GameObject tracerEffectPrefab = null;
		public static float maxDistance = 30f;
		public static float bounceDistance = 15f;
		public static float baseEntryDuration = 0.6f;
		public static float baseAttackDuration = 3f;
		public static float tickDamageCoefficient = 1.3333333333f;
		public static float procCoefficientPerTick = 1f;
		public static float baseTickFrequency = 3f;
		public static float force = 100f;

		public static string startAttackSoundString = "Play_mage_R_start";
		public static string endAttackSoundString = "Play_mage_R_end";
		public static string attackSoundString = "Play_mage_m1_cast_lightning";

		public static float recoilForce = 0f;

		//public static float effectResetDuration = 2.5f;
		//private float effectResetStopwatch;

		private static int baseTickCount = 20;
		//private int totalTickCount;
		private float tickFrequency;

		public int loadBaseTickCount;
		public float loadBaseTickFrequency;
		public float loadMaxDistance;

		private float flamethrowerStopwatch;
		private float stopwatch;
		private float entryDuration;
		private float flamethrowerDuration;
		private bool hasBegunAttack;
		private ChildLocator childLocator;
		private Transform leftFlamethrowerTransform;
		private Transform rightFlamethrowerTransform;
		private Transform leftMuzzleTransform;
		private Transform rightMuzzleTransform;
		private bool isCrit;
		private const float flamethrowerEffectBaseDistance = 16f;
	}
}