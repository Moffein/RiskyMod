using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using RiskyMod.Allies.DroneBehaviors;

namespace RiskyMod.Allies.DroneChanges
{
    public class MissileDrone
    {
        public MissileDrone()
        {
            GameObject missileDroneObject = AllyPrefabs.MissileDrone;
            missileDroneObject.AddComponent<AutoMissileBehavior>();

            CharacterBody cb = missileDroneObject.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 250f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;

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
