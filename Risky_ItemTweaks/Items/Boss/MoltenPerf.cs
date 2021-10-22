using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Risky_ItemTweaks.Items.Boss
{
    public class MoltenPerf
    {
        public static bool enabled = true;
        public static GameObject meatballPrefab;
        public static GameObject meatballEffect = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashFireMeatBall");

        public static void Modify()
        {
            if (!enabled || !Risky_ItemTweaks.disableProcChains) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireballsOnHit")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Effect handled in SharedHooks.OnHitEnemy

            meatballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FireMeatBall").InstantiateClone("RiskyItemTweaks_Perforator", true);
            ProjectileImpactExplosion pie = meatballPrefab.GetComponent<ProjectileImpactExplosion>();
            pie.blastProcCoefficient = 0f;
            ProjectileAPI.Add(meatballPrefab);
        }
    }
}
