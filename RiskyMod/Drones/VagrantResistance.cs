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
                AddBody("BackupDroneBody");

                AddBody("Drone1Body");
                AddBody("Drone2Body");
                AddBody("Turret1Body");

                AddBody("MissileDroneBody");
                AddBody("FlameDroneBody");
                AddBody("EquipmentDroneBody");
                AddBody("EmergencyDroneBody");

                VagrantIndex = BodyCatalog.FindBodyIndex("VagrantBody");
            };

            TakeDamage.ModifyInitialDamageActions += AddResist;
        }

        private static void AddResist(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (attackerBody.bodyIndex == VagrantResistance.VagrantIndex && damageInfo.procCoefficient > 1f && self.body && VagrantResistance.HasResist(self.body.bodyIndex))
            {
                damageInfo.procCoefficient *= 0.3333333333f;
                damageInfo.damage *= 0.3333333333f;
            }
        }

        public static bool HasResist(BodyIndex index)
        {
            foreach (BodyIndex b in AffectedBodies)
            {
                if (b == index) return true;
            }
            return false;
        }

        public static void AddBody(string bodyname)
        {
            BodyIndex index = BodyCatalog.FindBodyIndex(bodyname);
            if (index != BodyIndex.None)
            {
                AffectedBodies.Add(index);
            }
        }
    }
}
