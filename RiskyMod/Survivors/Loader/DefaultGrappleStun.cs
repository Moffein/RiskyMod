using RoR2;
using UnityEngine;
using R2API;
using RoR2.Projectile;
using EntityStates;

namespace RiskyMod.Survivors.Loader
{
    public class DefaultGrappleStun
    {
        public static bool enabled = true;
        public static GameObject GrappleHook;
        public DefaultGrappleStun()
        {
            if (!enabled) return;

            GrappleHook = Resources.Load<GameObject>("prefabs/projectiles/loaderyankhook").InstantiateClone("RiskyMod_LoaderHook", true);
            ProjectileGrappleController pgc = GrappleHook.GetComponent<ProjectileGrappleController>();
            pgc.yankMassLimit = 0f;
            pgc.ownerHookStateType = new SerializableEntityStateType(typeof(EntityStates.Loader.FireHook));

            /*ProjectileDamage pd = GrappleHook.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Stun1s | DamageType.NonLethal;*/

            ProjectileAPI.Add(GrappleHook);

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Loader.FireHook", "damageCoefficient", "3.2");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Loader.FireHook", "projectilePrefab", GrappleHook);
        }
    }
}
