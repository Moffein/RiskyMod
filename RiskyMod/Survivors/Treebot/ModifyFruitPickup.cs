using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Treebot
{
    public class ModifyFruitPickup
    {
        public static bool enabled = true;
        public ModifyFruitPickup()
        {
            if (!enabled) return;

            GameObject fruitPack = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotFruitPack.prefab").WaitForCompletion();

            GravitatePickup gp = fruitPack.GetComponentInChildren<GravitatePickup>();

            Collider pickupTrigger = gp.gameObject.GetComponent<Collider>();
            if (pickupTrigger && pickupTrigger.isTrigger)
            {
                pickupTrigger.transform.localScale *= 1.6f;
            }

            HealthPickup hp = fruitPack.GetComponentInChildren<HealthPickup>();
            hp.fractionalHealing = 0.25f;
        }
    }
}
