using RoR2.Orbs;
using RoR2;
using UnityEngine;
using System.Linq;
using EntityStates.Drone.DroneWeapon;
using UnityEngine.Networking;
using RoR2.CharacterAI;
using RiskyMod.Allies.DroneAutofireComponents;

namespace RiskyMod.Allies.DroneChanges
{
    public class MissileDrone
    {
        public MissileDrone()
        {
            GameObject missileDroneObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/missiledronebody");
            missileDroneObject.AddComponent<AutoMissileBehavior>();
            ModifyAI();
        }

        private void ModifyAI()
        {
            //Vanilla AI is terrible, just disable it and let the AutoMissileBehavior handle the shooting.
            GameObject masterObject = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/DroneMissileMaster");
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach(AISkillDriver skill in skillDrivers)
            {
                if (skill.skillSlot == SkillSlot.Primary)
                {
                    skill.skillSlot = SkillSlot.None;
                }
            }
        }
    }
}
