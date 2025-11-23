using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using RiskyMod.Allies.DroneChanges;
using R2API;
using UnityEngine.Networking;
using RiskyMod.Allies.DamageResistances;

namespace RiskyMod.Allies
{

    public class AlliesCore
    {
        public static bool enabled = true;

        public AlliesCore()
        {
            if (!enabled) return;

            new NoVoidDamage();
            new AntiSplat();

            TweakDrones();
        }

        private void TweakDrones()
        {
            new GunnerTurret();
            new MegaDrone();
        }
    }
}
