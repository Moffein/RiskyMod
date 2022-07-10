using UnityEngine;
using RoR2.CharacterAI;
using RoR2;

namespace RiskyMod.Enemies.Mobs
{
    public class Imp
    {
        public static bool enabled = true;
        public Imp()
        {
            if (!enabled) return;
            EnemiesCore.DisableRegen(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/impbody"));
            ModifyAI();
            RemoveSlashSlow();
            AddLunge();
        }

        private void ModifyAI()
        {
            GameObject enemyMaster = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/impmaster");

            AISkillDriver[] skills = enemyMaster.GetComponents<AISkillDriver>();
            foreach (AISkillDriver ai in skills)
            {
                //Instead of strafing, just chase the target.
                if (ai.customName == "StrafeBecausePrimaryIsntReady")
                {
                    //ai.skillSlot = RoR2.SkillSlot.Utility;
                    //ai.requireSkillReady = true;
                    ai.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
                    //ai.aimType = AISkillDriver.AimType.AtCurrentEnemy;
                    //ai.noRepeat = true;
                }
            }
        }

        private void RemoveSlashSlow()
        {
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.ImpMonster.DoubleSlash");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.ImpMonster.DoubleSlash", "walkSpeedPenaltyCoefficient", "1");   //0.5 is vanilla
        }

        private void AddLunge()
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.ImpMonster.DoubleSlash", "selfForce", "1200");  //Clay Man is 1800. This runs 2x.
        }
    }
}
