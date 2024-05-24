using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace RiskyMod.Moon
{
    public class FixVoidTeamBrotherEncounter
    {
        public static bool enabled = true;

        public FixVoidTeamBrotherEncounter()
        {
            if (!enabled) return;

            On.EntityStates.Missions.BrotherEncounter.BrotherEncounterBaseState.KillAllMonsters += BrotherEncounterBaseState_KillAllMonsters;
        }

        private void BrotherEncounterBaseState_KillAllMonsters(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterBaseState.orig_KillAllMonsters orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterBaseState self)
        {
            orig(self);

            if (NetworkServer.active)
            {
                foreach (TeamComponent teamComponent in new List<TeamComponent>(TeamComponent.GetTeamMembers(TeamIndex.Void)))
                {
                    if (teamComponent)
                    {
                        HealthComponent component = teamComponent.GetComponent<HealthComponent>();
                        if (component)
                        {
                            component.Suicide(null, null, DamageType.Generic);
                        }
                    }
                }
            }
        }
    }
}
