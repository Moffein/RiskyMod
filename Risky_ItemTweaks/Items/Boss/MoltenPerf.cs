using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace Risky_ItemTweaks.Items.Boss
{
    public class MoltenPerf
    {
        public static bool enabled = true;
        public static GameObject meatballPrefab;

        public static void Modify()
        {
            if (!enabled || !Risky_ItemTweaks.disableProcChains) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                /*c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireballsOnHit")
                    );*/
                c.GotoNext(
                    x => x.MatchLdstr("Prefabs/Projectiles/FireMeatBall")
                    );
                c.Index += 2;
                c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                {
                    return MoltenPerf.meatballPrefab;
                });
            };

            //Effect handled in SharedHooks.OnHitEnemy

            meatballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FireMeatBall").InstantiateClone("RiskyItemTweaks_Perforator", true);
            ProjectileImpactExplosion pie = meatballPrefab.GetComponent<ProjectileImpactExplosion>();
            pie.blastProcCoefficient = 0f;
            ProjectileAPI.Add(meatballPrefab);
        }
    }
}
