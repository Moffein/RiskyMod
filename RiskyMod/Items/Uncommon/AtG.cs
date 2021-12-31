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
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Missile);

			float initialDamage = initialDamageCoefficient - stackDamageCoefficient;

			IL.RoR2.GlobalEventManager.ProcMissile += (il) =>
			{
				ILCursor c = new ILCursor(il);
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
						 x => x.MatchLdfld<GlobalEventManager>("missilePrefab")
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
				missilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile").InstantiateClone("RiskyMod_ATGProjectile", true);
				ProjectileController pc = missilePrefab.GetComponent<ProjectileController>();
				pc.procCoefficient = 0f;
				ProjectileAPI.Add(missilePrefab);
			}
			else
            {
				missilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");

			}
		}
    }
}
