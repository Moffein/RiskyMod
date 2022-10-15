using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class OnHitAll
    {
        public delegate void OnHitAllDelegate(GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject);
        public static OnHitAllDelegate HandleOnHitAllActions;

        public static void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            orig(self, damageInfo, hitObject);
            if (!NetworkServer.active || damageInfo.procCoefficient == 0f || damageInfo.rejected)
            {
                return;
            }
            if (HandleOnHitAllActions != null) HandleOnHitAllActions.Invoke(self, damageInfo, hitObject);
        }
    }
}
