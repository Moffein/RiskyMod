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
            new AntiOneshot();
            new AntiVoidDeath();
            new AntiOverheat();

            TweakDrones();
        }

        private void TweakDrones()
        {
            new GunnerTurret();
            new MegaDrone();
        }

        public static bool IsDrone(HealthComponent health)
        {
            return health.body && !health.body.isPlayerControlled && health.body.teamComponent && health.body.teamComponent.teamIndex == RoR2.TeamIndex.Player
                && health.body.IsDrone;
                //&& ((health.body.bodyFlags & CharacterBody.BodyFlags.UsesAmbientLevel) > 0)
                //    || (health.body.inventory && health.body.inventory.GetItemCountPermanent(RoR2Content.Items.UseAmbientLevel) > 0);
        }
    }
}
