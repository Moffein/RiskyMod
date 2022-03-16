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
        public static List<string> AllyBodyNames = new List<string>
        {
            "BackupDroneBody",
            "Drone1Body",
            "Drone2Body",
            "Turret1Body",
            "MissileDroneBody",
            "FlameDroneBody",
            "EquipmentDroneBody",
            "EmergencyDroneBody",
            "MegaDroneBody",
            "SquidTurretBody",
            "BeetleGuardAllyBody",
            "RoboBallGreenBuddyBody",
            "RoboBallRedBuddyBody"
        };

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
            AllyBodies = new List<BodyIndex>();
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();

                foreach (string str in AllyBodyNames)
                {
                    AddBody(str);
                }

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
