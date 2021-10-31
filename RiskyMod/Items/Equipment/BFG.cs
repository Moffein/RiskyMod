using UnityEngine;
using R2API;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace RiskyMod.Items.Equipment
{
    public class BFG
    {
        public static bool enabled = true;
        public static GameObject projectilePrefab;
        public BFG()
        {
            if (!enabled) return;
            ItemsCore.ChangeEquipmentCooldown(ItemsCore.LoadEquipmentDef("bfg"), 120f);

            projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/BeamSphere").InstantiateClone("RiskyMod_BFG", true);
            ProjectileProximityBeamController pbc = projectilePrefab.GetComponent<ProjectileProximityBeamController>();
            pbc.damageCoefficient = 1.9f;
            ProjectileAPI.Add(projectilePrefab);

            LanguageAPI.Add("EQUIPMENT_BFG_DESC", "Fires preon tendrils, zapping enemies within 35m for up to <style=cIsDamage>1180% damage/second</style>. On contact, detonate in an enormous 20m explosion for <style=cIsDamage>8000% damage</style>.");

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
