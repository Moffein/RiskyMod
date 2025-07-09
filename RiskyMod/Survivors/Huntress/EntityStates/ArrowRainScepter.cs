using R2API;
using RiskyMod;
using RiskyMod.Survivors.Huntress;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.RiskyMod.Huntress
{
	//Was having trouble with certain things being inaccessible when inheriting ArrowRain so I just copypasted the skill.
    public class ArrowRainScepter : EntityStates.Huntress.BaseArrowBarrage
    {
        private void LoadStats()
        {
            this.maxDuration = 1.5f;
            this.beginLoopSoundString = "Play_huntress_R_aim_loop";
            this.endLoopSoundString = "Stop_huntress_R_aim_loop";
            this.fireSoundString = "Play_huntress_R_rain_start";
        }

		public override void OnEnter()
		{
			LoadStats();
			base.OnEnter();
			base.PlayAnimation("FullBody, Override", "LoopArrowRain");
			if (areaIndicatorPrefab)
			{
				this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(areaIndicatorPrefab);
				this.areaIndicatorInstance.transform.localScale = new Vector3(arrowRainRadius, arrowRainRadius, arrowRainRadius);
			}
		}

		private void UpdateAreaIndicator()
		{
			if (this.areaIndicatorInstance)
			{
				float maxDistance = 1000f;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxDistance, LayerIndex.world.mask))
				{
					this.areaIndicatorInstance.transform.position = raycastHit.point;
					this.areaIndicatorInstance.transform.up = raycastHit.normal;
				}
			}
		}

		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		public override void HandlePrimaryAttack()
		{
			base.HandlePrimaryAttack();
			this.shouldFireArrowRain = true;
			this.outer.SetNextStateToMain();
		}

		public void DoFireArrowRain()
		{
			EffectManager.SimpleMuzzleFlash(muzzleFlashEffect, base.gameObject, "Muzzle", false);
			if (this.areaIndicatorInstance && this.shouldFireArrowRain)
			{
				DamageTypeCombo dtc = new DamageTypeCombo()
				{
					damageType = DamageType.IgniteOnHit | DamageType.SlowOnHit,
					damageSource = DamageSource.Special
				};
				dtc.AddModdedDamageType(SharedDamageTypes.ProjectileRainForce);

                ProjectileManager.instance.FireProjectile(projectilePrefab, this.areaIndicatorInstance.transform.position,
					this.areaIndicatorInstance.transform.rotation, base.gameObject, this.damageStat * damageCoefficient, 0f,
					Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f,
					dtc);
			}
		}

		public override void OnExit()
		{
			if (this.shouldFireArrowRain && !this.outer.destroying)
			{
				this.DoFireArrowRain();
			}
			if (this.areaIndicatorInstance)
			{
				EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			}
			base.OnExit();
		}

		public static float arrowRainRadius = 7.5f * 1.5f;
		public static float damageCoefficient = 4.2f;
		public static GameObject projectilePrefab;
		public static GameObject areaIndicatorPrefab;
		public static GameObject muzzleFlashEffect;

		private GameObject areaIndicatorInstance;
		private bool shouldFireArrowRain;
	}
}
