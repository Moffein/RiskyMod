using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Treebot
{
    public class ModifyFruitPickup
    {
        public static bool enabled = true;
        public ModifyFruitPickup()
        {
            if (!enabled) return;

            GameObject fruitPack = Resources.Load<GameObject>("Prefabs/NetworkedObjects/TreebotFruitPack");

            GravitatePickup gp = fruitPack.GetComponentInChildren<GravitatePickup>();

            Collider pickupTrigger = gp.gameObject.GetComponent<Collider>();
            if (pickupTrigger && pickupTrigger.isTrigger)
            {
                pickupTrigger.transform.localScale *= 1.25f;
            }

            HealthPickup hp = fruitPack.GetComponentInChildren<HealthPickup>();
            hp.fractionalHealing = 0.2f;
        }
    }
}
