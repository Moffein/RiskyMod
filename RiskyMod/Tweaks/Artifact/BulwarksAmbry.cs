using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    public class BulwarksAmbry
    {
        public static bool enabled = true;

        public static int killCount = 0;

        public BulwarksAmbry()
        {
            if (!enabled) return;

            //Reset killcount on stage start
            On.RoR2.Stage.Start += (orig, self) =>
            {
                killCount = 0;
                orig(self);
            };

            On.RoR2.ArtifactTrialMissionController.CombatState.OnCharacterDeathGlobal += ArtifactKeyDrop;

            //Increase base dropchance
            On.RoR2.ArtifactTrialMissionController.OnStartServer += (orig, self) =>
            {
                orig(self);
                if (self.chanceForKeyDrop < 0.04f)
                {
                    self.chanceForKeyDrop = 0.04f;
                }
            };
        }

        private void ArtifactKeyDrop(On.RoR2.ArtifactTrialMissionController.CombatState.orig_OnCharacterDeathGlobal orig, EntityStates.EntityState self, DamageReport damageReport)
        {
            ArtifactTrialMissionController.ArtifactTrialMissionControllerBaseState artifactState = self as ArtifactTrialMissionController.ArtifactTrialMissionControllerBaseState;
            bool willDrop = Util.CheckRoll(Util.GetExpAdjustedDropChancePercent(artifactState.missionController.chanceForKeyDrop * 100f, damageReport.victim.gameObject), 0f, null);
            int maxKills = Mathf.CeilToInt(1f / artifactState.missionController.chanceForKeyDrop);
            //Debug.Log("Max Kills: " + maxKills);

            BulwarksAmbry.killCount++;
            if (BulwarksAmbry.killCount > maxKills)
            {
                //Debug.Log("Hit kill limit. Dropping key.");
                willDrop = true;
            }

            if (willDrop)
            {
                //Debug.Log("Running overwrite code.");
                BulwarksAmbry.killCount = 0;
                Debug.LogFormat("Creating artifact key pickup droplet.", Array.Empty<object>());
                PickupDropletController.CreatePickupDroplet(artifactState.missionController.GenerateDrop(), damageReport.victimBody.corePosition, Vector3.up * 20f);
            }
        }
    }
}
