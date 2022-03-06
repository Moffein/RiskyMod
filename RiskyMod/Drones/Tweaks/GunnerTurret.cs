using RoR2;
using UnityEngine;

namespace RiskyMod.Drones
{
    public class GunnerTurret
    {
        public GunnerTurret()
        {
            GameObject gunnerTurret = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/turret1body");
            SkillLocator sk = gunnerTurret.GetComponent<SkillLocator>();
            sk.primary.skillFamily.variants[0].skillDef.baseMaxStock = 1;
            sk.primary.skillFamily.variants[0].skillDef.baseRechargeInterval = 0f;
        }
    }
}
