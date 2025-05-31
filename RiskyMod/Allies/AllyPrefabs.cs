using UnityEngine;
using RoR2;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies
{
    public static class AllyPrefabs
    {
        public static GameObject GunnerTurret = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Turret1Body.prefab").WaitForCompletion();
        public static GameObject GunnerDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Drone1Body.prefab").WaitForCompletion();
        public static GameObject HealDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Drone2Body.prefab").WaitForCompletion();
        public static GameObject MissileDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/MissileDroneBody.prefab").WaitForCompletion();
        public static GameObject EquipmentDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/EquipmentDroneBody.prefab").WaitForCompletion();
        public static GameObject EmergencyDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/EmergencyDroneBody.prefab").WaitForCompletion();
        public static GameObject IncineratorDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/FlameDroneBody.prefab").WaitForCompletion();
        public static GameObject MegaDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/MegaDroneBody.prefab").WaitForCompletion();

        //Probably inefficient
        public static List<GameObject> PurchaseableDroneMasterList = new List<GameObject> 
        {
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Turret1Master.prefab").WaitForCompletion(),
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Drone1Master.prefab").WaitForCompletion(),
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Drone2Master.prefab").WaitForCompletion(),
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/DroneMissileMaster.prefab").WaitForCompletion(),
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/EquipmentDroneMaster.prefab").WaitForCompletion(),
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/EmergencyDroneMaster.prefab").WaitForCompletion(),
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/FlameDroneMaster.prefab").WaitForCompletion(),
            Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/MegaDroneMaster.prefab").WaitForCompletion()
        };

        public static bool IsPurchaseableDrone(GameObject bodyPrefab)
        {
            return PurchaseableDroneMasterList.Contains(bodyPrefab);
        }
    }
}
