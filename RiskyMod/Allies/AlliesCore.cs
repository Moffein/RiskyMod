using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskyMod.Allies
{
    public class AlliesCore
    {
        public static bool enabled = true;
        public static bool nerfDroneParts = true;

        public static List<AllyInfo> AllyList = new List<AllyInfo>();

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
            "DroneCommanderBody",

            "BeetleGuardAllyBody",
            "RoboBallGreenBuddyBody",
            "RoboBallRedBuddyBody",
            "Turret1Body",
            "SquidTurretBody"
        };

        public delegate void ModifyAllies(List<AllyInfo> allyList);
        public static ModifyAllies ModifyAlliesActions; //Runs after BodyCatalog init

        public AlliesCore()
        {
            if (!enabled) return;
            if (nerfDroneParts)
            {

            }
            BuildAllyBodies();
            new AllyScaling();
            new DroneTargeting();
            new IncreaseShotRadius();
            new SuperAttackResist();
            new NoVoidDamage();
            TweakDrones();
        }

        private void BuildAllyBodies()
        {

            /*Debug.Log("FireGatling: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.FireGatling", "damageCoefficient"));
            Debug.Log("FireMegaTurret: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.FireMegaTurret", "damageCoefficient"));
            Debug.Log("FireMissileBarrage: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.FireMissileBarrage", "damageCoefficient"));
            Debug.Log("FireTurret: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.FireTurret", "damageCoefficient"));
            Debug.Log("FireTwinRocket: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.FireTwinRocket", "damageCoefficient"));
            Debug.Log("Flamethrower: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.Flamethrower", "totalDamageCoefficient"));
            Debug.Log("Flamethrower: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.Flamethrower", "tickDamageCoefficient"));
            Debug.Log("HealBeam: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.Drone.DroneWeapon.HealBeam", "healCoefficient"));*/

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();

                foreach (string str in AllyBodyNames)
                {
                    AddBodyInternal(str);
                }

                if (ModifyAlliesActions != null) ModifyAlliesActions.Invoke(AllyList);
            };
        }

        private void TweakDrones()
        {
            new GunnerTurret();
            new MegaDrone();
        }

        public static bool AddBody(string bodyname, AllyTag tags)
        {
            bool addedSuccessfully = false;
            BodyIndex index = BodyCatalog.FindBodyIndex(bodyname);
            if (index != BodyIndex.None)
            {
                //Don't allow duplicates
                foreach (AllyInfo a in AllyList)
                {
                    if (a.bodyIndex == index)
                    {
                        return false;
                    }
                }

                AllyInfo ally = new AllyInfo
                {
                    bodyName = bodyname,
                    bodyIndex = index,
                    tags = tags
                };
                addedSuccessfully = true;
            }

            return addedSuccessfully;
        }

        //This one has preset info about each ally
        private bool AddBodyInternal(string bodyname)
        {
            bool addedSuccessfully = false;
            BodyIndex index = BodyCatalog.FindBodyIndex(bodyname);
            if (index != BodyIndex.None)
            {
                //Don't allow duplicates
                foreach (AllyInfo a in AllyList)
                {
                    if (a.bodyIndex == index)
                    {
                        return false;
                    }
                }

                AllyInfo ally = new AllyInfo
                {
                    bodyName = bodyname,
                    bodyIndex = index
                };
                switch (bodyname)
                {
                    case "BackupDroneBody":
                    case "Drone1Body":
                    case "Drone2Body":
                    case "MissileDroneBody":
                    case "FlameDroneBody":
                    case "EquipmentDroneBody":
                    case "EmergencyDroneBody":
                    case "MegaDroneBody":
                        ally.tags = AllyTag.Drone;
                        break;
                    case "DroneCommanderBody":
                        ally.tags = AllyTag.Drone | AllyTag.Item;
                        break;
                    case "BeetleGuardAllyBody":
                    case "RoboBallGreenBuddyBody":
                    case "RoboBallRedBuddyBody":
                        ally.tags = AllyTag.Item;
                        break;
                    case "Turret1Body":
                        ally.tags = AllyTag.Drone | AllyTag.Turret;
                        break;
                    case "SquidTurretBody":
                        ally.tags = AllyTag.Item | AllyTag.Turret;
                        break;
                    default:
                        break;
                }
                addedSuccessfully = true;
            }

            return addedSuccessfully;
        }

        public static bool IsAlly(BodyIndex bodyIndex)
        {
            bool flag = false;
            foreach (AllyInfo ally in AlliesCore.AllyList)
            {
                if (ally.bodyIndex == bodyIndex)
                {
                    return true;
                }
            }
            return flag;
        }

        public static bool IsTurretAlly(BodyIndex bodyIndex)
        {
            bool flag = false;
            foreach (AllyInfo ally in AlliesCore.AllyList)
            {
                if (ally.bodyIndex == bodyIndex)
                {
                    if ((ally.tags & AllyTag.Turret) == AllyTag.Turret) flag = true;
                    break;
                }
            }
            return flag;
        }
    }

    public class AllyInfo
    {
        public BodyIndex bodyIndex = BodyIndex.None;
        public string bodyName;
        public AllyTag tags = AllyTag.None;
    }

    public enum AllyTag
    {
        None,
        Drone,  //Benefits from Droneman
        Item,   //Is an item effect
        Turret,  //Resistance to AOE/Proc
        DontModifyScaling
    }
}
