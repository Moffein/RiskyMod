using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Allies.DroneChanges
{
    public class HealDrone
    {
        public HealDrone()
        {

            GameObject healDroneObject = AllyPrefabs.HealDrone;

            //Gets run before scaling changes
            CharacterBody healDroneBody = healDroneObject.GetComponent<CharacterBody>();
            healDroneBody.baseMaxHealth = 170f;
            healDroneBody.levelMaxHealth = healDroneBody.baseMaxHealth * 0.3f;
        }
    }
}
