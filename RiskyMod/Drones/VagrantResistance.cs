using RiskyMod.SharedHooks;
using RoR2;
using System.Collections.Generic;

namespace RiskyMod.Drones
{
    public class VagrantResistance
    {
        public static bool enabled = true;
        public static List<BodyIndex> AffectedBodies;
        public static BodyIndex VagrantIndex;

        public VagrantResistance()
        {
            if (!enabled) return;

            AffectedBodies = new List<BodyIndex>();
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();

                VagrantIndex = BodyCatalog.FindBodyIndex("VagrantBody");
            };

            TakeDamage.ModifyInitialDamageActions += AddResist;
        }

        private static void AddResist(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (!self.body.isPlayerControlled
                && attackerBody.bodyIndex == VagrantResistance.VagrantIndex 
                & damageInfo.procCoefficient > 1f
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && DronesCore.IsAlly(self.body.bodyIndex))
            {
                damageInfo.procCoefficient *= 0.5f;
                damageInfo.damage *= 0.5f;
            }
        }
    }
}
