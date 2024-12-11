﻿using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.SharedHooks
{
    public class HealthComponent_UpdateLastHitTime
    {
        public delegate void UpdateLastHitTimeDelegate(HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker);
        public static UpdateLastHitTimeDelegate UpdateLastHitTimeActions;

        public delegate void UpdateLastHitTimeInventoryDelegate(HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker, Inventory inventory);
        public static UpdateLastHitTimeInventoryDelegate UpdateLastHitTimeInventoryActions;

        public static void UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker, bool delayedDamage, bool firstHitOfDelayedDamage)
        {
            orig(self, damageValue, damagePosition, damageIsSilent, attacker, delayedDamage, firstHitOfDelayedDamage);
            if (NetworkServer.active && self.body && damageValue > 0f)
            {
                UpdateLastHitTimeActions?.Invoke(self, damageValue, damagePosition, damageIsSilent, attacker);

                if (self.body.inventory)
                {
                    if (UpdateLastHitTimeInventoryActions != null) UpdateLastHitTimeInventoryActions.Invoke(self, damageValue, damagePosition, damageIsSilent, attacker, self.body.inventory);
                }
            }
        }
    }
}
