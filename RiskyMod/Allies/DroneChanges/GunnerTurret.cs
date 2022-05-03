using RoR2;
using UnityEngine;

namespace RiskyMod.Allies.DroneChanges
{
    public class GunnerTurret
    {
        public GunnerTurret()
        {
            GameObject gunnerTurret = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/turret1body");
            SkillLocator sk = gunnerTurret.GetComponent<SkillLocator>();
            sk.primary.skillFamily.variants[0].skillDef.baseMaxStock = 1;
            sk.primary.skillFamily.variants[0].skillDef.baseRechargeInterval = 0f;

            //Gets run before scaling changes
            CharacterBody cb = gunnerTurret.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 240f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
        }
    }
}
