using UnityEngine;
using RoR2.CharacterAI;

namespace RiskyMod.Enemies.Mobs
{
    public class Imp
    {
        public static bool enabled = true;
        public Imp()
        {
            if (!enabled) return;
            EnemiesCore.DisableRegen(Resources.Load<GameObject>("prefabs/characterbodies/impbody"));
            ModifyAI();
        }

        private void ModifyAI()
        {
            GameObject enemyMaster = Resources.Load<GameObject>("prefabs/charactermasters/impmaster");
            AISkillDriver[] skills = enemyMaster.GetComponents<AISkillDriver>();

            foreach (AISkillDriver ai in skills)
            {
                if (ai.customName == "StrafeBecausePrimaryIsntReady")
                {
                    ai.skillSlot = RoR2.SkillSlot.Utility;
                    ai.requireSkillReady = true;
                    ai.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
                    ai.aimType = AISkillDriver.AimType.AtMoveTarget;
                    ai.noRepeat = true;
                }
            }
        }
    }
}
