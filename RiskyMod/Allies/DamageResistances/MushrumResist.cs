using RoR2;
using R2API;
using RiskyMod.SharedHooks;

namespace RiskyMod.Allies
{
    public class MushrumResist
    {
        public static bool enabled = true;
        public static BodyIndex MushrumBodyIndex;
        public MushrumResist()
        {
            if (!enabled) return;
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                MushrumBodyIndex = BodyCatalog.FindBodyIndex("MiniMushroomBody");
            };
            TakeDamage.ModifyInitialDamageActions += AddResist;
        }
        private static void AddResist(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (!self.body.isPlayerControlled
                && attackerBody.bodyIndex == MushrumBodyIndex
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && AlliesCore.IsTurretAlly(self.body.bodyIndex))
            {
                damageInfo.procCoefficient *= 0.3333333333f;
                damageInfo.damage *= 0.3333333333f;
            }
        }
    }
}
