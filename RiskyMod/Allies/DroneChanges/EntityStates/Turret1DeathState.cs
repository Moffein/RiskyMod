using RiskyMod;
using RiskyMod.Allies;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.Turret1
{
    public class Turret1DeathState : EntityStates.Drone.DeathState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active && !SoftDependencies.SS2_CheckDroneMarker(base.gameObject))
            {
                base.OnImpactServer(base.transform.position);
            }

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                //modelTransform.gameObject.SetActive(false);
                Destroy(modelTransform.gameObject);
            }
            base.Explode();
        }
    }
}
