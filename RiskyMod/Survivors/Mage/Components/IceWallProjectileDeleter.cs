using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Mage.Components
{
    public class IceWallProjectileDeleter : MonoBehaviour
    {
        BoxCollider collider;

        public void Awake()
        {
            collider = base.GetComponent<BoxCollider>();
            if (collider)
            {
                Debug.Log("Collider found");
            }
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (collider)
                {

                }
            }
        }
    }
}
