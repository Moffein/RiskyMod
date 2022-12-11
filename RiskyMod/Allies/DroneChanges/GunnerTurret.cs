using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class GunnerTurret
    {
        public GunnerTurret()
        {
            GameObject gunnerTurret = AllyPrefabs.GunnerTurret;
            SkillLocator sk = gunnerTurret.GetComponent<SkillLocator>();

            SkillDef turretSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Drones/Turret1BodyTurret.asset").WaitForCompletion();
            turretSkill.baseMaxStock = 1;
            turretSkill.baseRechargeInterval = 0f;

            //Gets run before scaling changes
            CharacterBody cb = gunnerTurret.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 300f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMaxShield = cb.baseMaxHealth * 0.1f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;
        }
    }
}
