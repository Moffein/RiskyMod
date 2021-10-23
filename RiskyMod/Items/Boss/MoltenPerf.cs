using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace Risky_Mod.Items.Boss
{
    public class MoltenPerf
    {
        public static bool enabled = true;
        public static GameObject meatballPrefab;

        public static void Modify()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_LIGHTNINGSTRIKEONHIT_DESC", "<style=cIsDamage>10%</style> chance on hit to down a lightning strike, dealing <style=cIsDamage>500%</style> <style=cStack>(+300% per stack)</style> damage.");

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
