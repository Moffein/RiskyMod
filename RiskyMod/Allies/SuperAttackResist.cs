using RiskyMod.SharedHooks;
using RoR2;
using R2API;
using RiskyMod.Tweaks.CharacterMechanics;

namespace RiskyMod.Allies
{
    public class SuperAttackResist
    {
        public static bool enabled = true;

        public SuperAttackResist()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageActions += AddResist;
        }

        private static void AddResist(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (!self.body.isPlayerControlled
                && damageInfo.HasModdedDamageType(ShieldGating.IgnoreShieldGateDamage)
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && AlliesCore.IsAlly(self.body.bodyIndex))
            {
                damageInfo.procCoefficient *= 0.5f;
                damageInfo.damage *= 0.5f;
            }
        }
    }
}
