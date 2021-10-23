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

            //Effect handled in SharedHooks.RecalculateStats
            
            //Buff lifetime and pickup range
            GameObject ammoPack = Resources.Load<GameObject>("Prefabs/NetworkedObjects/AmmoPack");

            DestroyOnTimer dt = ammoPack.GetComponent<DestroyOnTimer>();
            dt.duration = 20f;

            BeginRapidlyActivatingAndDeactivating br = ammoPack.GetComponent<BeginRapidlyActivatingAndDeactivating>();
            br.delayBeforeBeginningBlinking = dt.duration - 2f;

            GravitatePickup gp = ammoPack.GetComponentInChildren<GravitatePickup>();

            Collider pickupTrigger = gp.gameObject.GetComponent<Collider>();
            if (pickupTrigger && pickupTrigger.isTrigger)
            {
                pickupTrigger.transform.localScale *= 2f;
            }

            LanguageAPI.Add("ITEM_BANDOLIER_DESC", "<style=cIsUtility>Reduce skill cooldowns</style> by <style=cIsUtility>15%</style>. <style=cIsUtility>18%</style> + <style=cStack>(10% on stack)</style> chance on kill to drop an ammo pack that <style=cIsUtility>resets all cooldowns</style>.");
        }
    }
}
