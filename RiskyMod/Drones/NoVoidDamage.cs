using RoR2;
using UnityEngine.Networking;
using UnityEngine;

namespace RiskyMod.Drones
{
    public class NoVoidDamage
    {
        public static bool enabled = true;
        public NoVoidDamage()
        {
            if (!enabled) return;
            //null attacker/inflictor
            //damagecolor void
            //bypassblock bypassarmor

            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                if (NetworkServer.active)
                {
                    if (!self.body.isPlayerControlled)
                    {
                        if (!damageInfo.attacker && !damageInfo.inflictor
                        && damageInfo.damageColorIndex == DamageColorIndex.Void
                        && damageInfo.damageType == (DamageType.BypassArmor | DamageType.BypassBlock)
                        && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player))
                        {
                            if (DronesCore.IsAlly(self.body.bodyIndex))
                            {
                                damageInfo.rejected = true;
                            }
                        }
                    }
                }
                orig(self, damageInfo);
            };
        }
    }
}
