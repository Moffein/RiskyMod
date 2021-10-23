using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace RiskyMod.Items.Boss
{
    public class MoltenPerf
    {
        public static bool enabled = true;
        public static float initialDamageCoefficient = 3f;
        public static float stackDamageCoefficient = 1.8f;
        public static GameObject meatballPrefab;

        public MoltenPerf()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_FIREBALLSONHIT_DESC", "<style=cIsDamage>10%</style> chance on hit to call forth <style=cIsDamage>3 magma balls</style> from an enemy, dealing <style=cIsDamage>"+ItemsCore.ToPercent(initialDamageCoefficient)+"</style> <style=cStack>(+"+ ItemsCore.ToPercent(stackDamageCoefficient) + " per stack)</style> damage each.");

            float initialDamage = initialDamageCoefficient - stackDamageCoefficient;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireballsOnHit")
                    );
                c.GotoNext(
                     x => x.MatchLdfld<DamageInfo>("damage")
                    );
                c.Index -= 6;
                c.Next.Operand = 1.8f;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + 1.2f;
                });


                if (RiskyMod.disableProcChains)
                {
                    c.GotoNext(
                        x => x.MatchLdstr("Prefabs/Projectiles/FireMeatBall")
                        );
                    c.Index += 2;
                    c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                    {
                        return MoltenPerf.meatballPrefab;
                    });
                }
            };

            if (RiskyMod.disableProcChains)
            {
                meatballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FireMeatBall").InstantiateClone("RiskyItemTweaks_Perforator", true);
                ProjectileImpactExplosion pie = meatballPrefab.GetComponent<ProjectileImpactExplosion>();
                pie.blastProcCoefficient = 0f;
                ProjectileAPI.Add(meatballPrefab);
            }
        }
    }
}
