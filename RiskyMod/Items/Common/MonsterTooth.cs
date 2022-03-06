using UnityEngine;
using RoR2;

namespace RiskyMod.Items.Common
{
    public class MonsterTooth
    {
        public static bool enabled = true;
        public MonsterTooth()
        {
            if (!enabled) return;

            GameObject healPack = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/HealPack");

            //This doesn't stop the rolling.
            /*VelocityRandomOnStart vrs = healPack.GetComponent<VelocityRandomOnStart>();
            vrs.minSpeed = 10f;
            vrs.maxSpeed = 10f;*/

            //Buff lifetime and pickup range
            DestroyOnTimer dt = healPack.GetComponent<DestroyOnTimer>();
            dt.duration = 20f;

            BeginRapidlyActivatingAndDeactivating br = healPack.GetComponent<BeginRapidlyActivatingAndDeactivating>();
            br.delayBeforeBeginningBlinking = dt.duration - 2f;

            GravitatePickup gp = healPack.GetComponentInChildren<GravitatePickup>();
            gp.acceleration = 20f;

            Collider pickupTrigger = gp.gameObject.GetComponent<Collider>();
            if (pickupTrigger && pickupTrigger.isTrigger)
            {
                pickupTrigger.transform.localScale *= 2f;
            }

        }
    }
}
