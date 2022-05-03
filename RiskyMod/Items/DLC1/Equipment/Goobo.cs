using RoR2;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Equipment
{
    public class Goobo
    {
        public static bool enabled = true;
        public Goobo()
        {
            if (!enabled) return;
            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, DLC1Content.Equipment.GummyClone);
            };

            On.RoR2.Projectile.GummyCloneProjectile.SpawnGummyClone += (orig, self) =>
            {
                self.hpBoostCount = 60; //20 vanilla
                self.damageBoostCount = 30; //20 vanilla
                orig(self);
            };
        }
    }
}
