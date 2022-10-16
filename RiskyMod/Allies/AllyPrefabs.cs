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
        public static List<GameObject> PurchaseableDroneMasterList = new List<GameObject> 
        {
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/turret1master"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/drone1master"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/drone2master"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/missiledronemaster"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/equipmentdronemaster"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/emergencydronemaster"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/flamedronemaster"),
            LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/megadronemaster")
        };

        public static bool IsPurchaseableDrone(GameObject bodyPrefab)
        {
            return PurchaseableDroneMasterList.Contains(bodyPrefab);
        }
    }
}
