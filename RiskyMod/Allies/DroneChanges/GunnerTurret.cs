using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class GunnerTurret
    {
        public static bool allowRepair = true;
        public GunnerTurret()
        {
            GameObject gunnerTurret = AllyPrefabs.GunnerTurret;

            SkillDef turretSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Drones/Turret1BodyTurret.asset").WaitForCompletion();
            turretSkill.baseMaxStock = 1;
            turretSkill.baseRechargeInterval = 0f;

            if (GunnerTurret.allowRepair)
            {
                CharacterDeathBehavior cdb = gunnerTurret.GetComponent<CharacterDeathBehavior>();
                if (cdb)
                {
                    cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.Turret1.Turret1DeathState));
                }
            }
            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Turret1.Turret1DeathState));

            //Gets run before scaling changes
            CharacterBody cb = gunnerTurret.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 300f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMaxShield = cb.baseMaxHealth * 0.1f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;
        }
    }
}
