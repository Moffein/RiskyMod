using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.DLC1.Equipment
{
    public class Caffeinator
    {
        public static bool enabled = true;

        public Caffeinator()
        {
            if (!enabled) return;

            GameObject vendingPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VendingMachine/VendingMachine.prefab").WaitForCompletion();

            //Is DefaultLayer by default
            /*Debug.Log("Vending Layer: " + vendingPrefab.layer);
            Debug.Log("FakeActor Layer: " + LayerIndex.fakeActor.intVal);
            Debug.Log("NoCollision Layer: " + LayerIndex.noCollision.intVal);
            Debug.Log("Default Layer: " + LayerIndex.defaultLayer.intVal);*/

            CapsuleCollider cl = vendingPrefab.AddComponent<CapsuleCollider>();
            vendingPrefab.layer = LayerIndex.fakeActor.intVal;
        }
    }
}
