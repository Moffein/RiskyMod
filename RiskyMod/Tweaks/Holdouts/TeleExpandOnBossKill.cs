using RoR2;

namespace RiskyMod.Tweaks.Holdouts
{
    public class TeleExpandOnBossKill
    {
        public static bool enabled = true;
        public static bool enableDuringEclipse = false;
        public TeleExpandOnBossKill()
        {
            if (!enabled) return;

            On.RoR2.TeleporterInteraction.UpdateMonstersClear += (orig, self) =>
            {
                orig(self);
                //Minimum charge of 5% to prevent it from instantly expanding when the tele starts before boss is spawned
                if (self && self.monstersCleared && self.holdoutZoneController && self.activationState == TeleporterInteraction.ActivationState.Charging && self.chargeFraction > 0.05f)
                {
                    bool eclipseEnabled = Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse2;
                    if (enableDuringEclipse || !eclipseEnabled)
                    {
                        if (Util.GetItemCountForTeam(self.holdoutZoneController.chargingTeam, RoR2Content.Items.FocusConvergence.itemIndex, true, true) <= 0)
                        {
                            self.holdoutZoneController.currentRadius = 1000000f;
                        }
                    }
                }
            };
        }
    }
}
