using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class IncineratorDrone
    {
        public IncineratorDrone()
        {
            //Debug.Log("Dumping Drone flamer");
            //SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Drones/EntityStates.Drone.DroneWeapon.Flamethrower.asset").WaitForCompletion());
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Drones/EntityStates.Drone.DroneWeapon.Flamethrower.asset", "maxDistance", "25");    //12 vanilla
            SkillDef incineratorDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Drones/FlameDroneBodyFlamethrower.asset").WaitForCompletion();
            incineratorDef.baseRechargeInterval = 5f;   //10 vanilla

            CharacterBody body = AllyPrefabs.IncineratorDrone.GetComponent<CharacterBody>();
            body.baseMaxHealth = 340f;
            body.levelMaxHealth = body.baseMaxHealth * 0.3f;
            body.baseArmor = 20f;

            body.baseRegen = body.baseMaxHealth / 20f;
            body.levelRegen = body.baseRegen * 0.2f;
            body.baseMaxShield = body.baseMaxHealth * 0.08f;
            body.levelMaxShield = body.baseMaxShield * 0.3f;

            GameObject megaDroneBrokenObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/FlameDroneBroken.prefab").WaitForCompletion();
            PurchaseInteraction pi = megaDroneBrokenObject.GetComponent<PurchaseInteraction>();
            pi.cost = 80;	//Vanilla is 100
        }
    }
}
