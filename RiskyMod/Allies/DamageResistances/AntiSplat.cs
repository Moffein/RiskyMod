using RiskyMod.SharedHooks;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Allies.DamageResistances
{
    public class AntiSplat
    {
        public static bool enabled = true;
        
        public AntiSplat()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageActions += AddResist;
        }

        //This might be too general.
        private static void AddResist(DamageInfo damageInfo, HealthComponent self)
        {
            if (!self.body.isPlayerControlled
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && self.body.isFlying)
            {
                //Splat damage can have an attacker.
                //&& damageInfo.damageColorIndex == DamageColorIndex.Default    //Certain modded skills overwrite damagecolor
                if (damageInfo.inflictor == null
                    && damageInfo.damageType == DamageType.Generic
                    && damageInfo.procCoefficient == 0f
                    && damageInfo.dotIndex == DotController.DotIndex.None
                    && damageInfo.canRejectForce == true
                    && damageInfo.force == Vector3.zero)
                {
                    damageInfo.damage = 0f;
                    damageInfo.rejected = true;
                }
            }
        }
    }
}
