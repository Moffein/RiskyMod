using RoR2;
using UnityEngine;
using R2API;

namespace RiskyMod.Items.Uncommon
{
    public class Bandolier
    {
        public static bool enabled = true;
        public Bandolier()
        {
            if (!enabled) return;
            //Buff lifetime and pickup range
            GameObject ammoPack = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/AmmoPack");

            DestroyOnTimer dt = ammoPack.GetComponent<DestroyOnTimer>();
            dt.duration = 14f;

            BeginRapidlyActivatingAndDeactivating br = ammoPack.GetComponent<BeginRapidlyActivatingAndDeactivating>();
            br.delayBeforeBeginningBlinking = dt.duration - 2f;

            GravitatePickup gp = ammoPack.GetComponentInChildren<GravitatePickup>();

            Collider pickupTrigger = gp.gameObject.GetComponent<Collider>();
            if (pickupTrigger && pickupTrigger.isTrigger)
            {
                pickupTrigger.transform.localScale *= 2f;
            }
        }
    }
}
