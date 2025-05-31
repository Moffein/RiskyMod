﻿using RoR2;
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

            //Gets run before scaling changes
            CharacterBody gunnerDroneBody = AllyPrefabs.GunnerDrone.GetComponent<CharacterBody>();
            gunnerDroneBody.baseMaxHealth = 170f;
            gunnerDroneBody.levelMaxHealth = gunnerDroneBody.baseMaxHealth * 0.3f;

            AllyPrefabs.GunnerDrone.AddComponent<AutoGunnerDroneBehavior>();
            ModifyAI();
        }

        private void ModifyAI()
        {
            //Vanilla AI is terrible, just disable it and let the AutoGunnerDroneBehavior handle the shooting.
            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Drone1Master.prefab").WaitForCompletion();
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
