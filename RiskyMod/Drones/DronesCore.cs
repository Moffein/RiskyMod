using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;

namespace RiskyMod.Drones
{
    public class DronesCore
    {
        public static bool enabled = true;
        public static List<BodyIndex> AllyBodies = new List<BodyIndex>();
        public static List<BodyIndex> AllyTurretBodies = new List<BodyIndex>();
        public static List<string> AllyBodyNames = new List<string>
        {
            "BackupDroneBody",
            "Drone1Body",
            "Drone2Body",
            "MissileDroneBody",
            "FlameDroneBody",
            "EquipmentDroneBody",
            "EmergencyDroneBody",
            "MegaDroneBody",
            "BeetleGuardAllyBody",
            "RoboBallGreenBuddyBody",
            "RoboBallRedBuddyBody"
        };

        //These get added to the AllyBodies list, but they also get added to a separate list for referencing.
        public static List<string> AllyTurretBodyNames = new List<string>
        {
            "Turret1Body",
            "SquidTurretBody"
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
            new NoVoidDamage();
            TweakDrones();
        }

        private void BuildAllyBodies()
        {
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();

                foreach (string str in AllyBodyNames)
                {
                    AddBody(str);
                }
                foreach (string str in AllyTurretBodyNames)
                {
                    AddTurretBody(str);
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

        public static void AddTurretBody(string bodyname)
        {
            BodyIndex index = BodyCatalog.FindBodyIndex(bodyname);
            if (index != BodyIndex.None)
            {
                AllyBodies.Add(index);
                AllyTurretBodies.Add(index);
            }
        }

        public static bool IsAlly(BodyIndex bodyIndex)
        {
            return AllyBodies.Contains(bodyIndex);
        }

        //TODO: look into a more general tagging system instead of this
        public static bool IsTurretAlly(BodyIndex bodyIndex)
        {
            return AllyTurretBodies.Contains(bodyIndex);
        }
    }
}
