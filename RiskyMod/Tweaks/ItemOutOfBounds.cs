using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks
{
    public class ItemOutOfBounds
    {
        public static bool enabled = true;
        public ItemOutOfBounds()
        {
            if (!enabled) return;

            On.RoR2.GenericPickupController.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active) self.gameObject.AddComponent<PickupTracker>();
            };

            On.RoR2.PickupDropletController.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active) self.gameObject.AddComponent<PickupTracker>();
            };
        }

        public class PickupTracker : MonoBehaviour
        {
            public static float teleportTime = 10f;

            public Vector3 startingPoint;
            private float lifetime = 0f;

            public void Awake()
            {
                startingPoint = base.transform.position;
            }

            public void FixedUpdate()
            {
                lifetime += Time.fixedDeltaTime;
                if (lifetime > PickupTracker.teleportTime)
                {
                    Rigidbody rb = base.GetComponent<Rigidbody>();
                    if (rb)
                    {
                        if (rb.velocity.sqrMagnitude > 9f)
                        {
                            AttemptTeleport();
                        }
                    }
                    Destroy(this);
                }
            }

            public void AttemptTeleport()
            {
                TeleportHelper.TeleportGameObject(base.gameObject, SneedUtils.SneedUtils.FindSafeTeleportPosition(base.gameObject, startingPoint));
            }
        }
    }
}
