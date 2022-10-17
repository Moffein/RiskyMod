using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class HealthComponent_UpdateLastHitTime
    {
        public delegate void UpdateLastHitTimeDelegate(HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker);
        public static UpdateLastHitTimeDelegate UpdateLastHitTimeActions;

        public static void UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
        {
            orig(self, damageValue, damagePosition, damageIsSilent, attacker);
            if (NetworkServer.active && self.body && damageValue > 0f)
            {
                if (UpdateLastHitTimeActions != null) UpdateLastHitTimeActions.Invoke(self, damageValue, damagePosition, damageIsSilent, attacker);
            }
        }
    }
}
