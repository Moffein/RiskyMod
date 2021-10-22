using RoR2;
using UnityEngine;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;

namespace Risky_Mod.Items.Uncommon
{
    class AtG
    {
        public static bool enabled = true;

		public static GameObject missilePrefab;

        public static void Modify()
        {
            if (!enabled) return;

            On.RoR2.GlobalEventManager.ProcMissile += (orig, self, stack, attackerBody, attackerMaster, attackerTeamIndex, procChainMask, victim, damageInfo) =>
            {
				if (stack > 0)
				{
					GameObject gameObject = attackerBody.gameObject;
					InputBankTest component = gameObject.GetComponent<InputBankTest>();
					Vector3 position = component ? component.aimOrigin : gameObject.transform.position;
					Vector3 vector = component ? component.aimDirection : gameObject.transform.forward;
					Vector3 up = Vector3.up;
					if (Util.CheckRoll(10f * damageInfo.procCoefficient, attackerMaster))
					{
						float damageCoefficient = 1.2f + 1.8f * stack;
						float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);
						ProcChainMask procChainMask2 = procChainMask;
						procChainMask2.AddProc(ProcType.Missile);
						FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
						{
							projectilePrefab = RiskyMod.disableProcChains ? AtG.missilePrefab : self.missilePrefab,
							position = position,
							rotation = Util.QuaternionSafeLookRotation(up),
							procChainMask = procChainMask2,
							target = victim,
							owner = gameObject,
							damage = damage,
							crit = damageInfo.crit,
							force = 200f,
							damageColorIndex = DamageColorIndex.Item
						};
						ProjectileManager.instance.FireProjectile(fireProjectileInfo);
					}
				}
			};

			LanguageAPI.Add("ITEM_MISSILE_DESC", "<style=cIsDamage>10%</style> chance to fire a missile that deals <style=cIsDamage>300%</style> <style=cStack>(+180% per stack)</style> TOTAL damage.");
        
			if (RiskyMod.disableProcChains)
			{
				missilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile").InstantiateClone("RiskyItemTweaks_ATGProjectile", true);
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
