using RoR2;
using UnityEngine;

namespace RiskyMod.Allies.DroneChanges
{
    public class GunnerTurret
    {
        public GunnerTurret()
        {
            GameObject gunnerTurret = AllyPrefabs.GunnerTurret;
            SkillLocator sk = gunnerTurret.GetComponent<SkillLocator>();
            sk.primary.skillFamily.variants[0].skillDef.baseMaxStock = 1;
            sk.primary.skillFamily.variants[0].skillDef.baseRechargeInterval = 0f;

            //Gets run before scaling changes
            CharacterBody cb = gunnerTurret.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 240f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMaxShield = cb.baseMaxHealth * 0.08f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;
        }
    }
}
