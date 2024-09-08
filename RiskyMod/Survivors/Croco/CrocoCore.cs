using RiskyMod.Survivors.Croco.Tweaks;
using RiskyMod.Survivors.Croco.Contagion;
using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Croco
{
    public class CrocoCore
    {
        public static bool enabled = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody");

        public CrocoCore()
        {
            if (!enabled) return;

            new UtilityKnockdown();
            new BuffFrenziedLeap();
            new BuffShiftPuddleProc();
            new BiggerLeapHitbox();
            new ShiftAirControl();
            new ContagionPassive();
        }
    }
}
