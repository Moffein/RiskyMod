using System;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Void
{
    public class Zoea
    {
        public static bool enabled = true;
        public static bool ignoreAllyCap = true;
        public Zoea()
        {
            if (!enabled) return;
        }
    }
}
