using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;
using R2API;
using System;
using RoR2;
using UnityEngine.AddressableAssets;

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
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(GlobalEventManager.CommonAssets), "daggerPrefab")
                    ))
                {
                    c.Index++;
                    c.EmitDelegate<Func<GameObject, GameObject>>((oldPrefab) =>
                    {
                        return CeremonialDagger.daggerPrefab;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: CeremonialDagger IL Hook failed");
                }
            };

            daggerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Dagger/DaggerProjectile.prefab").WaitForCompletion();//.InstantiateClone("RiskyMod_CeremonialDaggerProjectile", true);
            ProjectileController pc = daggerPrefab.GetComponent<ProjectileController>();
            pc.procCoefficient = 0f;
            //Content.Content.projectilePrefabs.Add(daggerPrefab);
        }
    }
}
