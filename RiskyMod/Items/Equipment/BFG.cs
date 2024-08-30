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
        public BFG()
        {
            if (!enabled) return;
            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, RoR2Content.Equipment.BFG);
            };

            ItemsCore.ChangeEquipmentCooldown(ItemsCore.LoadEquipmentDef("bfg"), 120f);
        }
    }
}
