using RoR2;
using UnityEngine;

namespace RiskyMod.Fixes
{
    public class PreventArtifactHeal
    {
        public static bool enabled = true;
        public PreventArtifactHeal()
        {
            if (!enabled) return;

            //Based on https://thunderstore.io/package/rob2/ArtifactReliquaryHealingFix/
            On.RoR2.HealthComponent.Heal += (orig, self, amount, procChainMask, nonRegen) =>
            {
                if (self.body && self.body.baseNameToken == "ARTIFACTSHELL_BODY_NAME")
                {
                    amount = 0;
                }
                return orig(self, amount, procChainMask, nonRegen);
            };
        }
    }
}
