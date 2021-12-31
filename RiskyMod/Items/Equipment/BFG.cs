using UnityEngine;
using R2API;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RoR2;

namespace RiskyMod.Items.Equipment
{
    public class BFG
    {
        public static bool enabled = true;
        public static GameObject projectilePrefab;
        public BFG()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, RoR2Content.Equipment.BFG);

            ItemsCore.ChangeEquipmentCooldown(ItemsCore.LoadEquipmentDef("bfg"), 120f);

            projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/BeamSphere").InstantiateClone("RiskyMod_BFG", true);
            ProjectileProximityBeamController pbc = projectilePrefab.GetComponent<ProjectileProximityBeamController>();
            pbc.damageCoefficient = 1.9f;
            ProjectileAPI.Add(projectilePrefab);

            IL.RoR2.EquipmentSlot.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdstr("Prefabs/Projectiles/BeamSphere")
                    );
                c.Index+= 2;
                c.EmitDelegate<Func<GameObject, GameObject>>((projectile) =>
                {
                    return projectilePrefab;
                });
            };
        }
    }
}
