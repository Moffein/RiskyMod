using Risky_ItemTweaks.Items.Uncommon;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using RoR2.Projectile;
using Risky_ItemTweaks.Items.Common;
using Risky_ItemTweaks.Items.Boss;
using RoR2.Orbs;
using System.Collections.Generic;
using Risky_ItemTweaks.Items.Legendary;
using HG;
using Risky_ItemTweaks.Tweaks;

namespace Risky_ItemTweaks.SharedHooks
{
    class OnHitEnemy
    {
        public static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
			CharacterBody attackerBody = null;
			CharacterBody victimBody = null;

			if (NetworkServer.active && damageInfo.procCoefficient > 0f && !damageInfo.rejected)
			{
				if (damageInfo.attacker)
				{
					attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
					victimBody = victim ? victim.GetComponent<CharacterBody>() : null;

					if (FixDamageTypeOverwrite.enabled)
					{
						if ((damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic)
						{
							DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, 4f * damageInfo.procCoefficient, 1f);
						}
						if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic || attackerBody.HasBuff(RoR2Content.Buffs.AffixRed))
						{
							DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.PercentBurn, 4f * damageInfo.procCoefficient, 1f);
						}
					}
				}
			}

			orig(self, damageInfo, victim);

            if (NetworkServer.active && damageInfo.procCoefficient > 0f && !damageInfo.rejected)
            {
                if (damageInfo.attacker)
                {
                    if (attackerBody && victimBody)
					{
						Vector3 aimOrigin = attackerBody.aimOrigin;
						CharacterMaster attackerMaster = attackerBody.master;
						TeamComponent attackerTeamComponent = attackerBody.teamComponent;
						TeamIndex attackerTeamIndex = attackerTeamComponent ? attackerTeamComponent.teamIndex : TeamIndex.None;

						if (attackerMaster)
                        {
                            Inventory attackerInventory = attackerMaster.inventory;

                            if (LeechingSeed.enabled)
                            {
								if (!damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
								{
									int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.Seed);
									if (itemCount > 0)
									{
										HealthComponent attackerHealthComponent = attackerBody.GetComponent<HealthComponent>();
										if (attackerHealthComponent)
										{
											ProcChainMask procChainMask = damageInfo.procChainMask;
											procChainMask.AddProc(ProcType.HealOnHit);
											attackerHealthComponent.Heal(Mathf.Max(attackerBody.maxHealth * (float)itemCount * 0.005f, 1f * itemCount) * damageInfo.procCoefficient, procChainMask, true);
											//attackerBody.maxHealth * (0.01f + (float)(itemCount - 1) * 0.005f) 
										}
									}
								}
                            }
							if (ElementalBands.enabled)
                            {
								int iceRingCount = attackerInventory.GetItemCount(RoR2Content.Items.IceRing);
								int fireRingCount = attackerInventory.GetItemCount(RoR2Content.Items.FireRing);
								if (damageInfo.damage / attackerBody.damage >= 4f && attackerBody.HasBuff(RoR2Content.Buffs.ElementalRingsReady))
								{
									attackerBody.RemoveBuff(RoR2Content.Buffs.ElementalRingsReady);
									int num5 = 1;
									while ((float)num5 <= 10f)
									{
										attackerBody.AddTimedBuff(RoR2Content.Buffs.ElementalRingsCooldown, (float)num5);
										num5++;
									}
									ProcChainMask procChainMask4 = damageInfo.procChainMask;
									procChainMask4.AddProc(ProcType.Rings);


									Vector3 position2 = damageInfo.position;

									if (iceRingCount > 0)
									{
										float damageCoefficient = 2.5f + 1.5f * (float)(iceRingCount - 1);
										float damage2 = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);
										DamageInfo damageInfo2 = new DamageInfo
										{
											damage = damage2,
											damageColorIndex = DamageColorIndex.Item,
											damageType = DamageType.Generic,
											attacker = damageInfo.attacker,
											crit = damageInfo.crit,
											force = Vector3.zero,
											inflictor = null,
											position = position2,
											procChainMask = procChainMask4,
											procCoefficient = 1f
										};
										EffectManager.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/IceRingExplosion"), position2, Vector3.up, true);
										victimBody.AddTimedBuff(RoR2Content.Buffs.Slow80, 3f * (float)iceRingCount);
										victimBody.healthComponent.TakeDamage(damageInfo2);
									}
									if (fireRingCount > 0)
									{
										GameObject gameObject = Resources.Load<GameObject>("Prefabs/Projectiles/FireTornado");
										float resetInterval = gameObject.GetComponent<ProjectileOverlapAttack>().resetInterval;
										float lifetime = gameObject.GetComponent<ProjectileSimple>().lifetime;
										float damageCoefficient = 3.5f + 2.1f * (float)(fireRingCount - 1);
										float damage3 = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient) / lifetime * resetInterval;
										float speedOverride = 0f;
										Quaternion rotation2 = Quaternion.identity;
										Vector3 vector = position2 - aimOrigin;
										vector.y = 0f;
										if (vector != Vector3.zero)
										{
											speedOverride = -1f;
											rotation2 = Util.QuaternionSafeLookRotation(vector, Vector3.up);
										}
										ProjectileManager.instance.FireProjectile(new FireProjectileInfo
										{
											damage = damage3,
											crit = damageInfo.crit,
											damageColorIndex = DamageColorIndex.Item,
											position = position2,
											procChainMask = procChainMask4,
											force = 0f,
											owner = damageInfo.attacker,
											projectilePrefab = gameObject,
											rotation = rotation2,
											speedOverride = speedOverride,
											target = null
										});
									}
								}
							}
							if (Shatterspleen.enabled)
                            {
								if (!damageInfo.procChainMask.HasProc(ProcType.BleedOnHit))
								{
									int daggerCount = attackerInventory.GetItemCount(RoR2Content.Items.BleedOnHit);
									int spleenCount = attackerInventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode);
									bool flag = (damageInfo.damageType & DamageType.BleedOnHit) > DamageType.Generic;
									if ((daggerCount + spleenCount > 0 || flag) && (flag || Util.CheckRoll((10f * daggerCount + (spleenCount > 0 ? 5f : 0f)) * damageInfo.procCoefficient, attackerMaster)))
									{
										ProcChainMask procChainMask2 = damageInfo.procChainMask;
										procChainMask2.AddProc(ProcType.BleedOnHit);
										DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Bleed, 3f * damageInfo.procCoefficient, 1f);
									}
								}
							}

							//Recalculate Death Mark since some debuffs are being moved to outside of OnHitEnemy.
							int deathMarkCount = attackerMaster.inventory.GetItemCount(RoR2Content.Items.DeathMark);
							int debuffCount = 0;
							if (deathMarkCount >= 1 && !victimBody.HasBuff(RoR2Content.Buffs.DeathMark))
							{
								foreach (BuffIndex buffType in BuffCatalog.debuffBuffIndices)
								{
									if (victimBody.HasBuff(buffType))
									{
										debuffCount++;
									}
								}
								DotController dotController = DotController.FindDotController(victim.gameObject);
								if (dotController)
								{
									for (DotController.DotIndex dotIndex = DotController.DotIndex.Bleed; dotIndex < DotController.DotIndex.Count; dotIndex++)
									{
										if (dotController.HasDotActive(dotIndex))
										{
											debuffCount++;
										}
									}
								}
								if (debuffCount >= 4)
								{
									victimBody.AddTimedBuff(RoR2Content.Buffs.DeathMark, 7f * (float)deathMarkCount);
								}
							}
						}
                    }
                }
            }
        }
    }
}
