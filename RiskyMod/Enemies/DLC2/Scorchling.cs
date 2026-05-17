using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.DLC2
{
    public class Scorchling
    {
        public static bool enabled = true;

        public Scorchling()
        {
            if (!enabled) return;

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/Scorchling/EntityStates.Scorchling.LavaBomb.asset", "mortarDamageCoefficient", "0.8");
        }
    }
}
