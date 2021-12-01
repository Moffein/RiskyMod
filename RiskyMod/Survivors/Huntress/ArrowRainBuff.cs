using RoR2;
using UnityEngine;
using RoR2.Projectile;
using R2API;

namespace RiskyMod.Survivors.Huntress
{
    public class ArrowRainBuff
    {
        public static bool enabled = true;
        public static GameObject arrowRainObject;

        public ArrowRainBuff()
        {
            if (!enabled) return;

            arrowRainObject = Resources.Load<GameObject>("prefabs/projectiles/huntressarrowrain").InstantiateClone("RiskyModArrowRainProjectile", true);
            ProjectileAPI.Add(arrowRainObject);

            ProjectileDotZone arrowDotZone = arrowRainObject.GetComponent<ProjectileDotZone>();
            arrowDotZone.overlapProcCoefficient = 0.7f;
            arrowDotZone.damageCoefficient = 1f / 3f; //Adjusts DPS value to match the entitystate damagecoefficient

            HitBox hb = arrowRainObject.GetComponentInChildren<HitBox>();
            hb.transform.localScale = new Vector3(hb.transform.localScale.x, 2f * hb.transform.localScale.y, hb.transform.localScale.z);
        }
    }
}
