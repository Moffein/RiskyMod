using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class EmergencyDrone
    {
        public EmergencyDrone()
        {
            CharacterBody body = AllyPrefabs.EmergencyDrone.GetComponent<CharacterBody>();
            body.baseMaxHealth = 360f;
            body.levelMaxHealth = body.baseMaxHealth * 0.3f;

            GameObject megaDroneBrokenObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/EmergencyDroneBroken.prefab").WaitForCompletion();
            PurchaseInteraction pi = megaDroneBrokenObject.GetComponent<PurchaseInteraction>();
            pi.cost = 80;	//Vanilla is 100
        }
    }
}
