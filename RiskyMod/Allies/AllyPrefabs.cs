using UnityEngine;
using RoR2;
using System.Collections.Generic;

namespace RiskyMod.Allies
{
    public static class AllyPrefabs
    {
        public static GameObject GunnerTurret = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/turret1body");
        public static GameObject GunnerDrone = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/drone1body");
        public static GameObject HealDrone = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/drone2body");
        public static GameObject MissileDrone = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/missiledronebody");
        public static GameObject EquipmentDrone = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/equipmentdronebody");
        public static GameObject EmergencyDrone = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/emergencydronebody");
        public static GameObject IncineratorDrone = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/flamedronebody");
        public static GameObject MegaDrone = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/megadronebody");

        //Probably inefficient
        public static List<GameObject> PurchaseableDroneList = new List<GameObject> 
        {
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/turret1body"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/drone1body"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/drone2body"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/missiledronebody"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/equipmentdronebody"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/emergencydronebody"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/flamedronebody"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/megadronebody")
        };

        public static bool IsPurchaseableDrone(GameObject bodyPrefab)
        {
            return PurchaseableDroneList.Contains(bodyPrefab);
        }
    }
}
