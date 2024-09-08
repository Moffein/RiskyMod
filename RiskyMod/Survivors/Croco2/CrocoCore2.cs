using RiskyMod.Survivors.Croco2.Tweaks;
using RiskyMod.Survivors.Croco2.Contagion;
using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Croco2
{
    public class CrocoCore2
    {
        public static bool enabled = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody");

        public CrocoCore2()
        {
            if (!enabled) return;

            new BuffShiftPuddleProc();
            new BiggerLeapHitbox();
            new ShiftAirControl();
            new ContagionPassive();
        }
    }
}
