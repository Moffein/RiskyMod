using UnityEngine;
using RoR2;

namespace RiskyMod.Items.Uncommon
{
    public class Daisy
    {
        public static bool enabled = true;


        public Daisy()
        {
            if (!enabled) return;
        }
    }

    public class DaisyBehaviour : MonoBehaviour
    {
        public static GameObject wardPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/Shrines/ShrineHealingWard");
    }
}
