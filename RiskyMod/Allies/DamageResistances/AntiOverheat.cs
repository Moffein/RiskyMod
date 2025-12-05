using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DamageResistances
{
    public class AntiOverheat
    {
        public static bool enabled = true;
        public AntiOverheat()
        {
            if (!enabled) return;

            //OverheatSystem.maxStacksForDrones = 4;
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && AlliesCore.IsDrone(self) && self.body.HasBuff(RoR2Content.Buffs.Overheat) && damageInfo.procCoefficient <= 0f)
            {
                if (damageInfo.dotIndex == DotController.DotIndex.Burn || damageInfo.dotIndex == DotController.DotIndex.PercentBurn || damageInfo.dotIndex == DotController.DotIndex.StrongerBurn)
                {
                    damageInfo.damageType.damageType |= DamageType.NonLethal;
                }
            }
            orig(self, damageInfo);
        }
    }
}
