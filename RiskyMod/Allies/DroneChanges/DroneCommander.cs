using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class DroneCommander
    {
        public DroneCommander()
        {
            CharacterBody cb = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneCommander/DroneCommanderBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            cb.baseMaxHealth = 200f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMaxShield = cb.baseMaxHealth * 0.08f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;
        }
    }
}
