using RiskyMod.SharedHooks;
using RoR2;

namespace RiskyMod.Allies
{
    public class EliteDamageModifiers
    {
        public static bool enabled = true;
        public EliteDamageModifiers()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageActions += AddModifiers;
        }
        private static void AddModifiers(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            //Allies resist elites
            if (!self.body.isPlayerControlled
                && attackerBody.isElite
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && !(attackerBody.teamComponent && attackerBody.teamComponent.teamIndex == TeamIndex.Player)
                && AlliesCore.IsAlly(self.body.bodyIndex))
            {
                damageInfo.damage *= 0.6666666666f;
            }

            //Allies deal bonus damage against elites
            /*if (self.body.isElite && !attackerBody.isPlayerControlled
                && (attackerBody.teamComponent && attackerBody.teamComponent.teamIndex == TeamIndex.Player)
                && !(self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && AlliesCore.IsAlly(attackerBody.bodyIndex))
            {
                damageInfo.damage *= 1.5f;
            }*/
        }
    }
}
