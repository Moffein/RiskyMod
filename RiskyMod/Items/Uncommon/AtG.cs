using RoR2;
using UnityEngine;
using RoR2.Projectile;
using RoR2.Orbs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using System;

namespace RiskyMod.Items.Uncommon
{
    class AtG
    {
        public static bool enabled = true;
		public static bool useOrb = true;

		public static float initialDamageCoefficient = 3f;
		public static float stackDamageCoefficient = 1.8f;

		public static GameObject missilePrefab;

        public AtG()
        {
            if (!enabled) return;
			ItemsCore.ModifyItemDefActions += ModifyItem;

			float initialDamage = initialDamageCoefficient - stackDamageCoefficient;

			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				//Disable missiles if fired by a Player, since they'll be using an OrbAttack instead.
				c.GotoNext(MoveType.After,
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "Missile")
					);
				c.Emit(OpCodes.Ldloc, 4); //master
				c.EmitDelegate<Func<ItemDef, CharacterMaster, ItemDef>>((item, master) =>
				{
					if (useOrb && master.teamIndex == TeamIndex.Player) item = RiskyMod.emptyItemDef;
					return item;
				});

				c.GotoNext(
					 x => x.MatchLdcR4(3f)
					);
				c.Next.Operand = stackDamageCoefficient;

				c.GotoNext(
					 x => x.MatchMul()
					);
				c.Index++;
				c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
				{
					return damageCoefficient + initialDamage;
				});

				if (RiskyMod.disableProcChains)
				{
					c.GotoNext(
					 x => x.MatchLdsfld(typeof(GlobalEventManager.CommonAssets), "missilePrefab")
					);
					c.Index++;
					c.EmitDelegate<Func<GameObject, GameObject>>((projectilePrefab) =>
					{
						return missilePrefab;
					});
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
										missileOrb.target = mainHurtBox;
										OrbManager.instance.AddOrb(missileOrb);
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
