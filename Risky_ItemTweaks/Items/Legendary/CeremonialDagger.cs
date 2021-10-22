using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;
using R2API;

namespace Risky_ItemTweaks.Items.Legendary
{
    class CeremonialDagger
    {
        public static bool enabled = true;
        public static GameObject daggerPrefab;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Dagger")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Effect handled in SharedHooks.OnCharacterDeath

            if (Risky_ItemTweaks.disableProcChains)
            {
                daggerPrefab = Resources.Load<GameObject>("prefabs/projectiles/DaggerProjectile").InstantiateClone("RiskyItemTweaks_CeremonialDaggerProjectile", true);
                ProjectileController pc = daggerPrefab.GetComponent<ProjectileController>();
                pc.procCoefficient = Risky_ItemTweaks.disableProcChains ? 0f : 1f;
                ProjectileAPI.Add(daggerPrefab);
            }
            else
            {
                daggerPrefab = Resources.Load<GameObject>("prefabs/projectiles/DaggerProjectile");
            }
        }
    }
}
