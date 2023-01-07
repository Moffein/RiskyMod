using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.VoidFields
{
    public class FogRework
    {
        public static bool enabled = false;

        public FogRework()
        {
            if (!enabled) return;

            On.RoR2.ArenaMissionController.ReadyNextNullWard += DisableFogUntilRoundStart;
            On.RoR2.ArenaMissionController.BeginRound += ActivateFogOnRoundStart;
        }

        private void DisableFogUntilRoundStart(On.RoR2.ArenaMissionController.orig_ReadyNextNullWard orig, ArenaMissionController self)
        {
            orig(self);
            if (self.fogDamageInstance)
            {
                self.fogDamageInstance.SetActive(false);
            }
        }

        private void ActivateFogOnRoundStart(On.RoR2.ArenaMissionController.orig_BeginRound orig, RoR2.ArenaMissionController self)
        {
            orig(self);
            if (self.fogDamageInstance)
            {
                self.fogDamageInstance.SetActive(true);
            }
        }
    }
}
