using RoR2;
using UnityEngine;

namespace RiskyMod.Enemies
{
    public class MonsterFallDamage
    {
        public static bool enabled = true;
        public MonsterFallDamage()
        {
            if (!enabled) return;

            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool isPlayer = self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player;
                if (!isPlayer)
                {
                    if (damageInfo.damageType.HasFlag(DamageType.FallDamage) && damageInfo.damageType.HasFlag(DamageType.NonLethal))
                    {
                        damageInfo.damageType &= ~DamageType.NonLethal;
                        damageInfo.damageType |= DamageType.BypassOneShotProtection;
                    }
                }
                orig(self, damageInfo);
            };
        }
    }
}
