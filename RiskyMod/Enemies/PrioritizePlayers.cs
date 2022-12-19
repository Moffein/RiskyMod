using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace RiskyMod.Enemies
{
    public class PrioritizePlayers
    {
        //Enemies in the list will always try to prioritize targeting players.
        public static List<BodyIndex> prioritizePlayersList = new List<BodyIndex>();

        public PrioritizePlayers()
        {
            On.RoR2.CharacterAI.BaseAI.UpdateTargets += (orig, self) =>
            {
                orig(self);
                if (self.body && self.body.teamComponent && (prioritizePlayersList.Contains(self.body.bodyIndex)))
                {
                    if (self.currentEnemy != null
                    && self.currentEnemy.characterBody
                    && !self.currentEnemy.characterBody.isPlayerControlled
                    && self.currentEnemy.characterBody.teamComponent)
                    {
                        TeamMask enemyTeams = TeamMask.GetEnemyTeams(self.body.teamComponent.teamIndex);

                        List<CharacterBody> targetList = new List<CharacterBody>();
                        foreach (PlayerCharacterMasterController pc in PlayerCharacterMasterController.instances)
                        {
                            if (pc.body && pc.body.isPlayerControlled && pc.body.teamComponent &&  enemyTeams.HasTeam(pc.body.teamComponent.teamIndex) && pc.body.healthComponent && pc.body.healthComponent.alive)
                            {
                                targetList.Add(pc.body);
                            }
                        }

                        if (targetList.Count > 0)
                        {
                            Vector3 myPos = self.body.corePosition;
                            float shortestDistSqr = Mathf.Infinity;
                            CharacterBody newTarget = null;

                            foreach (CharacterBody cb in targetList)
                            {
                                float sqrDist = (myPos - cb.corePosition).sqrMagnitude;
                                if (sqrDist < shortestDistSqr)
                                {
                                    shortestDistSqr = sqrDist;
                                    newTarget = cb;
                                }
                            }

                            if (newTarget)
                            {
                                self.currentEnemy.gameObject = newTarget.gameObject;
                                self.currentEnemy.bestHurtBox = newTarget.mainHurtBox;
                                self.enemyAttention = self.enemyAttentionDuration;
                                self.targetRefreshTimer = 10f;
                                self.BeginSkillDriver(self.EvaluateSkillDrivers());
                            }
                        }
                    }
                }
            };
        }
    }
}
