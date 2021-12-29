using RoR2;
using UnityEngine;
using R2API;
using RoR2.Projectile;
using MonoMod.Cil;
using RiskyMod.MonoBehaviours;

namespace RiskyMod.Survivors.Loader
{
    public class PylonMagnet
    {
        public PylonMagnet()
        {
            //Cloning this causes the collision box to be offset above the projectile for some reason.
            GameObject pylon = Resources.Load<GameObject>("prefabs/projectiles/loaderpylon");//.InstantiateClone("RiskyMod_LoaderPylon", true);
            pylon = BuildProjectile(pylon);
            //ProjectileAPI.Add(PylonProjectile);
            SneedUtils.SneedUtils.SetEntityStateField("entitystates.loader.throwpylon", "projectilePrefab", pylon);
        }

        private GameObject BuildProjectile(GameObject go)
        {
            RadialForceMassLimited rf = go.AddComponent<RadialForceMassLimited>();
            rf.radius = 25f;
            rf.damping = 0.5f;
            rf.forceMagnitude = -250f;
            rf.forceCoefficientAtEdge = 0.5f;
            rf.maxMass = 250f;  //same as alt grapple

            return go;
        }
    }
}
