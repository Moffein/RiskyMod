using RoR2;
using UnityEngine;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using System;

namespace RiskyMod.Items.Uncommon
{
    class AtG
    {
        public static bool enabled = true;

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
				c.GotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "Missile")
					);

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
		private static void ModifyItem()
		{
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Missile);
		}
	}
}
