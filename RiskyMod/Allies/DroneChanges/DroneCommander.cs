using UnityEngine;
using RoR2;

namespace RiskyMod.Allies.DroneChanges
{
    public class DroneCommander
    {
        public DroneCommander()
        {
            CharacterBody cb = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/dronecommanderbody").GetComponent<CharacterBody>();
            cb.baseMaxHealth = 200f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMaxShield = cb.baseMaxHealth * 0.08f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;
        }
    }
}
