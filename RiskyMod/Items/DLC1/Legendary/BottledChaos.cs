using RoR2;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class BottledChaos
    {
        public static bool enabled = true;
        public BottledChaos()
        {
            if (!enabled) return;

            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();

                EquipmentCatalog.randomTriggerEquipmentList.Remove(RoR2Content.Equipment.FireBallDash.equipmentIndex);
                EquipmentCatalog.randomTriggerEquipmentList.Remove(RoR2Content.Equipment.Gateway.equipmentIndex);
                /*foreach (EquipmentIndex ed in EquipmentCatalog.randomTriggerEquipmentList)
                {
                    Debug.Log(EquipmentCatalog.GetEquipmentDef(ed).name);
                }*/
                /*[Info   : Unity Log] [RoR2.EquipmentCatalog] BFG
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Blackhole
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Cleanse
                [Info   : Unity Log] [RoR2.EquipmentCatalog] CommandMissile
                [Info   : Unity Log] [RoR2.EquipmentCatalog] CritOnUse
                [Info   : Unity Log] [RoR2.EquipmentCatalog] DeathProjectile
                [Info   : Unity Log] [RoR2.EquipmentCatalog] DroneBackup
                [Info   : Unity Log] [RoR2.EquipmentCatalog] FireBallDash
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Fruit
                [Info   : Unity Log] [RoR2.EquipmentCatalog] GainArmor
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Gateway
                [Info   : Unity Log] [RoR2.EquipmentCatalog] GummyClone
                [Info   : Unity Log] [RoR2.EquipmentCatalog] LifestealOnHit
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Lightning
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Molotov
                [Info   : Unity Log] [RoR2.EquipmentCatalog] PassiveHealing
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Saw
                [Info   : Unity Log] [RoR2.EquipmentCatalog] Scanner
                [Info   : Unity Log] [RoR2.EquipmentCatalog] TeamWarCry
                [Info   : Unity Log] [RoR2.EquipmentCatalog] VendingMachine*/
            };
        }
    }
}
