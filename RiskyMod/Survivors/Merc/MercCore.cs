using UnityEngine;
using RoR2;
using BepInEx.Configuration;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RiskyMod.Survivors.Merc
{
    public class MercCore
    {
        public static bool enabled = true;

        public static bool modifyStats = true;
        public static ConfigEntry<bool> m1ComboFinishTweak;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MercBody");
        public MercCore()
        {
            if (!enabled) return;
            //TODO: Buff default shift
            //Slightly buff Evis? Don't really feel like it needs it but wouldn't be opposed to it.
        }
    }
}
