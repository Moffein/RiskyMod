using RoR2;

namespace RiskyMod.Tweaks.RunScaling
{
    public class CombatDirectorMultiplier
    {
        public static bool enabled = true;
        public static float multiplier = 1.2f;

        public CombatDirectorMultiplier()
        {
            if (!enabled) return;

            On.RoR2.CombatDirector.Awake += (orig, self) =>
            {
                if (Run.instance.gameModeIndex != RiskyMod.simulacrumIndex)
                {
                    self.creditMultiplier *= multiplier;
                }
                orig(self);
            };
        }
    }
}
