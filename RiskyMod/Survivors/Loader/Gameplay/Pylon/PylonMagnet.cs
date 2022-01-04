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
            rf.maxMass = 400f;  //Greater wisps are 300. Yank hook is 250.
            rf.flyingOnly = true;

            go.AddComponent<PylonPulseController>();

            return go;
        }
    }

    public class PylonPulseController : MonoBehaviour
    {
        public static float initialDelay = 1.65f;
        public static float timeBetweenShocks = 1f;
        public static float pulseDuration = 0.4f;

        public RadialForceMassLimited radialForce;
        public float stopwatch;
        public float pulseTimer;

        bool hasStarted = false;

        public void Awake()
        {
            radialForce = base.GetComponent<RadialForceMassLimited>();
            pulseTimer = 0f;
            stopwatch = 0f;
            radialForce.enabled = false;
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (!hasStarted)
            {
                if (stopwatch > initialDelay)
                {
                    hasStarted = true;
                    radialForce.enabled = true;
                    stopwatch = 0f;
                    StartPulse();
                }
            }
            else
            {
                if (pulseTimer > 0f)
                {
                    pulseTimer -= Time.fixedDeltaTime;
                    if (pulseTimer <= 0f)
                    {
                        EndPulse();
                    }
                }
                if (stopwatch > timeBetweenShocks)
                {
                    stopwatch -= timeBetweenShocks;
                    StartPulse();
                }
            }
        }

        public void StartPulse()
        {
            pulseTimer = pulseDuration;
            radialForce.forceMagnitude = -750f;
        }

        public void EndPulse()
        {
            radialForce.forceMagnitude = -150f;
        }
    }
}
