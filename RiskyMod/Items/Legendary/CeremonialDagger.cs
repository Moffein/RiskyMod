using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;
using R2API;
using System;

namespace RiskyMod.Items.Legendary
{
    class CeremonialDagger
    {
        public static bool enabled = true;
        public static GameObject daggerPrefab;
        public CeremonialDagger()
        {
            if (!enabled || !RiskyMod.disableProcChains) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     //x => x.MatchLdsfld(typeof(RoR2Content.Items), "Dagger")
                     x => x.MatchLdfld<RoR2.GlobalEventManager>("daggerPrefab")
                    );
                c.Index++;
                c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                {
                    return CeremonialDagger.daggerPrefab;
                });
            };

            daggerPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/DaggerProjectile").InstantiateClone("RiskyMod_CeremonialDaggerProjectile", true);
            ProjectileController pc = daggerPrefab.GetComponent<ProjectileController>();
            pc.procCoefficient = RiskyMod.disableProcChains ? 0f : 1f;
            ProjectileAPI.Add(daggerPrefab);
        }
    }
}
