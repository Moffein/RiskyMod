using UnityEngine;

namespace RiskyMod.Items.DLC3.Legendary
{
    public class NetworkedSuffering
    {
        public static bool enabled = true;
        public NetworkedSuffering()
        {
            if (!enabled) return;

            SharedHooks.LanguageModifiers.ModifyLanguageTokenActions += ModifyLang;
            On.RoR2.SharedSufferingManager.OnEnable += SharedSufferingManager_OnEnable;
        }

        private void SharedSufferingManager_OnEnable(On.RoR2.SharedSufferingManager.orig_OnEnable orig, RoR2.SharedSufferingManager self)
        {
            orig(self);
            self._damageSentToPool = 0.2f;
        }

        private string ModifyLang(string token, string localized)
        {
            if (token != "ITEM_SHAREDSUFFERING_DESC") return localized;
            return localized.Replace("45", "20");
        }
    }
}
