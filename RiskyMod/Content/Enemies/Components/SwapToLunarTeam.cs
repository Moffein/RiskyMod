using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Content.Enemies.Components
{
    public class SwapToLunarTeam : MonoBehaviour
    {
        public void Start()
        {
            if (NetworkServer.active)
            {
                TeamComponent tc = GetComponent<TeamComponent>();
                if (tc && tc.teamIndex == TeamIndex.Monster)
                {
                    tc.teamIndex = TeamIndex.Lunar;
                }

                CharacterBody cb = GetComponent<CharacterBody>();
                if (cb)
                {
                    if (cb.master && cb.master.teamIndex == TeamIndex.Monster)
                    {
                        cb.master.teamIndex = TeamIndex.Lunar;
                        if (cb.master.aiComponents != null)
                        {
                            foreach (BaseAI ai in cb.master.aiComponents)
                            {
                                ai.UpdateTargets();
                            }
                        }
                    }
                }
            }

            Destroy(this);
        }
    }
}
