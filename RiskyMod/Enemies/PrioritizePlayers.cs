using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;
using RoR2.CharacterAI;

namespace RiskyMod.Enemies
{
    public class PrioritizePlayers
    {
        //Enemies in the list will always try to prioritize targeting players.
        public static List<BodyIndex> prioritizePlayersList = new List<BodyIndex>();

        public static void AttemptTargetPlayer(On.RoR2.CharacterAI.BaseAI.orig_UpdateTargets orig, BaseAI self)
        {
            orig(self);
            if (self.body && self.body.teamComponent && (prioritizePlayersList.Contains(self.body.bodyIndex)))
            {
                if (self.currentEnemy != null
                && self.currentEnemy.characterBody
                && !(IsPlayer(self.currentEnemy.characterBody))
                && self.currentEnemy.characterBody.teamComponent)
                {
                    TeamMask enemyTeams = TeamMask.GetEnemyTeams(self.body.teamComponent.teamIndex);

                    List<CharacterBody> targetList = new List<CharacterBody>();
                    foreach (PlayerCharacterMasterController pc in PlayerCharacterMasterController.instances)
                    {
                        if (pc.body && pc.body.isPlayerControlled && pc.body.teamComponent && enemyTeams.HasTeam(pc.body.teamComponent.teamIndex) && pc.body.healthComponent && pc.body.healthComponent.alive && ((int)pc.body.GetVisibilityLevel(self.body) >= (int)VisibilityLevel.Revealed))
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
        }

        private static bool IsPlayer(CharacterBody body)
        {
            //Watched Mithrix job to a single engi turret, don't have high hopes for his ability to kill them.
            /*bool isPlayerControlled = body.isPlayerControlled;
            bool isAmbientLevel = body.inventory && body.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel) > 0;

            bool isPlayer = isPlayerControlled || !isAmbientLevel;*/

            return body.isPlayerControlled && body.healthComponent && body.healthComponent.alive;
        }
    }
}
