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

            On.RoR2.TeleporterInteraction.UpdateMonstersClear += TeleporterInteraction_UpdateMonstersClear;
        }

        private void TeleporterInteraction_UpdateMonstersClear(On.RoR2.TeleporterInteraction.orig_UpdateMonstersClear orig, TeleporterInteraction self)
        {
            orig(self);
            //Minimum charge of 5% to prevent it from instantly expanding when the tele starts before boss is spawned
            if (self && self.monstersCleared && self.holdoutZoneController && self.activationState == TeleporterInteraction.ActivationState.Charging && self.chargeFraction > 0.05f)
            {
                bool eclipseEnabled = Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse2;
                if (enableDuringEclipse || !eclipseEnabled)
                {
                    bool isFocusConvergence = Util.GetTotalItemCountFromTeam(new Util.GetTotalItemCountFromTeamArgs(RoR2Content.Items.FocusConvergence.itemIndex)
                    {
                        requiresAlive = true,
                        requiredTeam = new TeamIndex?(self.holdoutZoneController.chargingTeam),
                        requiresConnected = true
                    }) > 0;

                    bool isPatience = Util.GetTotalItemCountFromTeam(new Util.GetTotalItemCountFromTeamArgs(DLC4Content.Items.OutOfTPRadius.itemIndex)
                    {
                        requiresAlive = true,
                        requiredTeam = new TeamIndex?(self.holdoutZoneController.chargingTeam),
                        requiresConnected = true
                    }) > 0;

                    if (!isFocusConvergence && !isPatience)
                    {
                        self.holdoutZoneController.currentRadius = 1000000f;
                    }
                }
            }
        }
    }
}
