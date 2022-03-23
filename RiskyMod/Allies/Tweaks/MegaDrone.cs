using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace RiskyMod.Allies
{
    public class MegaDrone
    {
        public MegaDrone()
        {
            GameObject megaDroneMasterObject = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/MegaDroneMaster");

            AISkillDriver[] aiDrivers = megaDroneMasterObject.GetComponentsInChildren<AISkillDriver>();
            for (int i = 0; i < aiDrivers.Length; i++)
            {
                if (aiDrivers[i].customName == "StopTooCloseTarget")
                {
                    aiDrivers[i].movementType = AISkillDriver.MovementType.StrafeMovetarget;
                    break;
                }
            }
        }
    }
}
