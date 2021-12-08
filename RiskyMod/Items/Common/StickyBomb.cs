using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RoR2.Projectile;
using UnityEngine;
using System;

namespace RiskyMod.Items.Common
{
    public class StickyBomb
    {
        public static bool enabled = true;
        public static GameObject stickybombPrefab;

        public static ItemDef itemDef = RoR2Content.Items.StickyBomb;

        public StickyBomb()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, itemDef);

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "StickyBomb")
                    );

                //New Damage
                c.GotoNext(
                    x => x.MatchLdcR4(1.8f)
                    );
                c.Next.Operand = 2.4f;

                //Replace projectile
                c.GotoNext(
                    x => x.MatchLdstr("Prefabs/Projectiles/StickyBomb")
                    );
                c.Index += 2;
                c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                {
                    return StickyBomb.stickybombPrefab;
                });
            };

            //Modify detonation delay
            stickybombPrefab = Resources.Load<GameObject>("prefabs/projectiles/stickybomb").InstantiateClone("RiskyMod_Stickybomb", true);
            ProjectileImpactExplosion pie = stickybombPrefab.GetComponent<ProjectileImpactExplosion>();
            pie.lifetime = 1.2f;
            ProjectileAPI.Add(stickybombPrefab);
        }
    }
}
