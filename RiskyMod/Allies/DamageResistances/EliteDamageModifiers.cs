using RiskyMod.SharedHooks;
using RoR2;

namespace RiskyMod.Allies
{
    //Unnecessary. Allies deal good damage now.
    public class EliteDamageModifiers
    {
        public static bool enabled = false;
        public EliteDamageModifiers()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageAttackerActions += AddModifiers;
        }
        private static void AddModifiers(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            //Allies resist elites
            if (!self.body.isPlayerControlled
                && attackerBody.isElite
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && !(attackerBody.teamComponent && attackerBody.teamComponent.teamIndex == TeamIndex.Player)
                && (self.body.inventory && self.body.inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0))
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
