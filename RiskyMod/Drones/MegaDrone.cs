using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace RiskyMod.Drones
{
    public class MegaDrone
    {
        public MegaDrone()
        {
            GameObject megaDroneMasterObject = Resources.Load<GameObject>("prefabs/charactermasters/MegaDroneMaster");

            AISkillDriver[] aiDrivers = megaDroneMasterObject.GetComponentsInChildren<AISkillDriver>();
            for (int i = 0; i < aiDrivers.Length; i++)
            {
                if (aiDrivers[i].customName == "StopTooCloseTarget")
                {
                    aiDrivers[i].movementType = AISkillDriver.MovementType.FleeMoveTarget;
                    break;
                }
            }
        }
    }
}
