using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;

namespace RiskyMod.Drones
{
    public class DronesCore
    {
        public static bool enabled = true;
        public static List<BodyIndex> AllyBodies;

        public delegate void ModifyAllies(List<BodyIndex> bodies);
        public static ModifyAllies ModifyAlliesActions;

        public DronesCore()
        {
            if (!enabled) return;
            BuildAllyBodies();
            new DroneScaling();
            new DroneTargeting();
            new IncreaseShotRadius();
            new VagrantResistance();
            TweakDrones();
        }

        private void BuildAllyBodies()
        {
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

                AddBody("MegaDroneBody");

                AddBody("SquidTurretBody");
                AddBody("BeetleGuardAllyBody");

                if (ModifyAlliesActions != null) ModifyAlliesActions.Invoke(AllyBodies);
            };
        }

        private void TweakDrones()
        {
            new GunnerTurret();
            new MegaDrone();
        }

        public static void AddBody(string bodyname)
        {
            BodyIndex index = BodyCatalog.FindBodyIndex(bodyname);
            if (index != BodyIndex.None)
            {
                AllyBodies.Add(index);
            }
        }

        public static bool IsAlly(BodyIndex bodyIndex)
        {
            return AllyBodies.Contains(bodyIndex);
        }
    }
}
