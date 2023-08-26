using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.MonoBehaviours
{
    [RequireComponent(typeof(BuffWard))]
    public class WardHeal : MonoBehaviour
    {
        private float stopwatch;
        private BuffWard ward;

        public float healFraction = 0.01f;
        public float healInterval = 1f;

        private void Awake()
        {
            ward = base.GetComponent<BuffWard>();
            stopwatch = 0f;
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= healInterval)
            {
                stopwatch -= healInterval;
                Heal();
            }
        }

        private void Heal()
        {
            if (!ward) return;

            float radiusSqr = ward.calculatedRadius * ward.calculatedRadius;
            Vector3 position = base.transform.position;
            if (ward.invertTeamFilter)
            {
                for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                {
                    if (teamIndex != ward.teamFilter.teamIndex)
                    {
                        HealTeam(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
                    }
                }
                return;
            }
            HealTeam(TeamComponent.GetTeamMembers(ward.teamFilter.teamIndex), radiusSqr, position);
        }

        private void HealTeam(System.Collections.ObjectModel.ReadOnlyCollection<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            foreach (TeamComponent teamComponent in recipients)
            {
                Vector3 vector = teamComponent.transform.position - currentPosition;
                if (ward.shape == BuffWard.BuffWardShape.VerticalTube)
                {
                    vector.y = 0f;
                }
                if (vector.sqrMagnitude <= radiusSqr)
                {
                    CharacterBody body = teamComponent.GetComponent<CharacterBody>();
                    if (body && body.healthComponent && (!ward.requireGrounded || !body.characterMotor || body.characterMotor.isGrounded))
                    {
                        body.healthComponent.HealFraction(healFraction, default);
                    }
                }
            }
        }
    }
}
