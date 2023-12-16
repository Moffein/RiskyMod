using RiskyMod.SharedHooks;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Allies.DamageResistances
{
    public class AntiSplat
    {
        public static bool enabled = true;
        
        public AntiSplat()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageNoAttackerActions += AddResist;
        }

        //This might be too general.
        private static void AddResist(DamageInfo damageInfo, HealthComponent self)
        {
            if (!self.body.isPlayerControlled
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && (self.body.inventory && self.body.inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0))
            {
                //Splat damage can have an attacker.
                if (damageInfo.inflictor == null
                    && damageInfo.damageColorIndex == DamageColorIndex.Default
                    && damageInfo.damageType == DamageType.Generic)
                {
                    damageInfo.damage = 0f;
                    damageInfo.rejected = true;
                }
            }
        }
    }
}
