using RoR2;
using UnityEngine;
using RoR2.Projectile;
using RoR2.Orbs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using System;
using System.Linq;

namespace RiskyMod.Items.Uncommon
{
    class AtG
    {
        public static bool enabled = true;

		public static bool useOrb = false;
		public static bool alwaysOrb = false;

		public static float initialDamageCoefficient = 3f;
		public static float stackDamageCoefficient = 2.1f;

		public static float projectileDamageTreshold = 4f;


		public static GameObject missilePrefab;

        public AtG()
        {
            if (!enabled) return;
			ItemsCore.ModifyItemDefActions += ModifyItem;

			float initialDamage = initialDamageCoefficient - stackDamageCoefficient;

			IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
			{
				bool error = true;

				ILCursor c = new ILCursor(il);

				//Disable missiles if fired by a Player, since they'll be using an OrbAttack instead.
				if (c.TryGotoNext(MoveType.After,
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "Missile")
					))
                {
					c.Emit(OpCodes.Ldloc, 2); //victimBody
					c.Emit(OpCodes.Ldloc, 5); //master
					c.Emit(OpCodes.Ldarg_1);    //damageinfo
					c.EmitDelegate<Func<ItemDef, CharacterBody, CharacterMaster, DamageInfo, ItemDef>>((item, victimBody, master, damageInfo) =>
					{
						if (useOrb && master.teamIndex == TeamIndex.Player)
						{
							if (alwaysOrb)
							{
								item = RiskyMod.emptyItemDef;
							}
							else if (victimBody.healthComponent && victimBody.healthComponent.alive)    //Only fire orb if target is alive to prevent whiffing against already-dead targets.
							{
								CharacterBody cb = master.GetBody();
								if (cb)
								{
									if (damageInfo.damage / cb.damage < projectileDamageTreshold)
									{
										item = RiskyMod.emptyItemDef;
									}
								}
							}
						}
						return item;
					});

					if (c.TryGotoNext(
						 x => x.MatchLdcR4(3f)
						))
					{
						c.Next.Operand = stackDamageCoefficient;

						if (c.TryGotoNext(
							 x => x.MatchMul()
							))
						{
							c.Index++;
							c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
							{
								return damageCoefficient + initialDamage;
							});

							if (RiskyMod.disableProcChains)
							{
								if(c.TryGotoNext(
								 x => x.MatchLdsfld(typeof(GlobalEventManager.CommonAssets), "missilePrefab")
								))
								{
									c.Index++;
									c.EmitDelegate<Func<GameObject, GameObject>>((projectilePrefab) =>
									{
										return missilePrefab;
									});
								}
							}
							error = false;
						}
					}
				}

				if (error)
                {
					UnityEngine.Debug.LogError("RiskyMod: AtG IL Hook failed");
				}
			};

			if (useOrb) SharedHooks.OnHitEnemy.OnHitAttackerInventoryActions += AtGOrb;

			if (RiskyMod.disableProcChains)
			{
				missilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MissileProjectile").InstantiateClone("RiskyMod_ATGProjectile", true);
				ProjectileController pc = missilePrefab.GetComponent<ProjectileController>();
				pc.procCoefficient = 0f;
				Content.Content.projectilePrefabs.Add(missilePrefab);
			}
			else
            {
				missilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");

			}
		}

		private static void AtGOrb(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory)
		{
			if (!damageInfo.procChainMask.HasProc(ProcType.Missile))
            {
				//Only players fire orbs. Enemies fire physical missiles so that you can evade them by using cover.
				//Based off of Plasma Shrimp code
				if (attackerBody.teamComponent && attackerBody.teamComponent.teamIndex == TeamIndex.Player)
                {
					int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.Missile);
					if (itemCount > 0)
                    {
						//Need this check to prevent heavy hits from rolling twice.
						if (damageInfo.damage / attackerBody.damage < projectileDamageTreshold)
                        {
							if (Util.CheckRoll(10f * damageInfo.procCoefficient, attackerBody.master))
							{
								float damageCoefficient = initialDamageCoefficient + stackDamageCoefficient * (itemCount - 1);
								float damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);

								int icbmCount = attackerInventory.GetItemCount(DLC1Content.Items.MoreMissile);
								if (icbmCount > 1)
								{
									damageValue *= 1f + 0.5f * (icbmCount - 1);
								}

								if (attackerBody.aimOrigin != null)
								{
									HurtBox targetHurtBox = victimBody.mainHurtBox;

									int missilesToFire = icbmCount > 0 ? 3 : 1;
									for (int i = 0; i < missilesToFire; i++)
									{
										MicroMissileOrb missileOrb = new MicroMissileOrb();
										missileOrb.origin = attackerBody.aimOrigin;
										missileOrb.damageValue = damageValue;
										missileOrb.isCrit = damageInfo.crit;
										missileOrb.teamIndex = attackerBody.teamComponent.teamIndex;
										missileOrb.attacker = damageInfo.attacker;
										missileOrb.procChainMask = damageInfo.procChainMask;
										missileOrb.procChainMask.AddProc(ProcType.Missile);
										missileOrb.procCoefficient = RiskyMod.disableProcChains ? 0f : 1f;
										missileOrb.damageColorIndex = DamageColorIndex.Item;
										HurtBox mainHurtBox = victimBody.mainHurtBox;
										if (mainHurtBox)
										{
											missileOrb.target = targetHurtBox;
											OrbManager.instance.AddOrb(missileOrb);
										}
									}
								}
							}
						}
					}
                }
            }
        }

		private static void ModifyItem()
		{
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Missile);
		}
	}
}
