using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Survivors.Croco2
{
    public class CrocoCore2
    {
        public static bool enabled = false;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody");

        public CrocoCore2()
        {
            if (!enabled) return;
        }
    }
}
