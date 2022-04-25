using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.RunScaling
{
    public class CombatDirectorMultiplier
    {
        public static bool enabled = true;
        public static float directorCreditMultiplier = 1.2f;
        public static float scaledGoldRatio = 1f;

        public CombatDirectorMultiplier()
        {
            if (!enabled) return;

            scaledGoldRatio = 1f / (Mathf.Sqrt(CombatDirectorMultiplier.directorCreditMultiplier));

            On.RoR2.CombatDirector.Awake += (orig, self) =>
            {
                if (Run.instance.gameModeIndex != RiskyMod.simulacrumIndex)
                {
                    self.creditMultiplier *= directorCreditMultiplier;
                }
                orig(self);
            };
        }
    }
}
