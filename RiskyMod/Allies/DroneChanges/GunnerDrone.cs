using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using RiskyMod.Allies.DroneBehaviors;
using UnityEngine.AddressableAssets;
using R2API;

namespace RiskyMod.Allies.DroneChanges
{
    public class GunnerDrone
    {
        public GunnerDrone()
        {
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.Drone.DroneWeapon.FireTurret");
            GameObject muzzleflash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/Muzzleflash1.prefab").WaitForCompletion().InstantiateClone("RiskyMod_GunnerDroneMuzzleflash",false);
            EffectComponent ec = muzzleflash.GetComponent<EffectComponent>();
            ec.soundName = "Play_drone_attack";
            Content.Content.effectDefs.Add(new EffectDef(muzzleflash));
            AutoGunnerDroneBehavior.fireEffectPrefab = muzzleflash;

            GameObject gunnerDroneObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/drone1body");
            gunnerDroneObject.AddComponent<AutoGunnerDroneBehavior>();

            //Gets run before scaling changes
            CharacterBody gunnerDroneBody = gunnerDroneObject.GetComponent<CharacterBody>();
            gunnerDroneBody.baseMaxHealth = 170f;
            gunnerDroneBody.levelMaxHealth = gunnerDroneBody.baseMaxHealth * 0.3f;

            ModifyAI();
        }

        private void ModifyAI()
        {
            //Vanilla AI is terrible, just disable it and let the AutoGunnerDroneBehavior handle the shooting.
            GameObject masterObject = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/Drone1Master");
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach (AISkillDriver skill in skillDrivers)
            {
                if (skill.skillSlot == SkillSlot.Primary)
                {
                    skill.skillSlot = SkillSlot.None;
                }
            }
        }
    }
}
