using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using RiskyMod.Allies.DroneChanges;

namespace RiskyMod.Allies
{
    public class AlliesCore
    {
        public static bool enabled = true;
        public static bool nerfDroneParts = true;
        public static bool dronePartsIgnoresAllyCap = true;
        public static bool beetleGlandDontRetaliate = true;

        public delegate void ModifyAllies(List<AllyInfo> allyList);
        public static ModifyAllies ModifyAlliesActions; //Runs after BodyCatalog init

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
            "SquidTurretBody",

            "MinorConstructAllyBody"
        };

        private void TweakDrones()
        {
            new GunnerTurret();
            new MegaDrone();
            new IncineratorDrone();
            new MissileDrone();
            new GunnerDrone();
            new HealDrone();
        }

        public AlliesCore()
        {
            if (!enabled) return;
            BuildAllyBodies();
            if (nerfDroneParts)
            {
                Items.ItemsCore.ModifyItemDefActions += ModifyDroneParts;
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.DroneWeaponsChainGun.FireChainGun", "damageCoefficient", "0.6");   //coef 1 orig

                IL.RoR2.DroneWeaponsBoostBehavior.OnEnemyHit += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchCallvirt<CharacterBody>("get_damage")
                        ))
                    {
                        c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                        c.EmitDelegate<Func<float, DamageInfo, float>>((bodyDamage, damageInfo) => damageInfo.damage);
                    }
                    else
                    {
                        Debug.LogError("RiskyMod: AlliesCore DroneWeaponsBoostBehavior.OnEnemyHit IL Hook failed");
                    }
                };
            }

            if (dronePartsIgnoresAllyCap)
            {
                IL.RoR2.DroneWeaponsBehavior.TrySpawnDrone += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(
                         x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                        ))
                    {
                        c.EmitDelegate<Func<DirectorSpawnRequest, DirectorSpawnRequest>>(spawnRequest =>
                        {
                            spawnRequest.ignoreTeamMemberLimit = dronePartsIgnoresAllyCap;
                            return spawnRequest;
                        });
                    }
                    else
                    {
                        Debug.LogError("RiskyMod: AlliesCore DroneWeaponsBehavior.TrySpawnDrone IL Hook failed");
                    }
                };
            }

            if (AllyScaling.normalizeDroneDamage)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTurret", "damageCoefficient", "0.25");   //Shared with Gunner Drones, but those use a dedicated component to handle attacking now

                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireGatling", "damageCoefficient", "0.45");   //Damage 18 -> 12, coef 0.3 -> 0.45
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.HealBeam", "healCoefficient", "1.7");   //Damage 10 -> 12, coef 2 -> 1.6667
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMegaTurret", "damageCoefficient", "2.2");   //Damage 14 -> 12, coef 2.2 -> 2.5667 mult by 0.8
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTwinRocket", "damageCoefficient", "4.6667");   //Damage 14 -> 12, coef 4 -> 4.6667

                //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMissileBarrage", "damageCoefficient", "1.7");   //Damage 14 -> 12, coef 1 -> 1.166666667
            }
            new AllyScaling();
            new DroneTargeting();
            new ModifyBulletAttacks();
            new SuperAttackResist();
            new MushrumResist();
            new EliteDamageModifiers();
            new NoVoidDamage();
            new StricterLeashing();
            TweakDrones();
        }

        private static void ModifyDroneParts()
        {
            HG.ArrayUtils.ArrayAppend(ref Items.ItemsCore.changedItemDescs, DLC1Content.Items.DroneWeapons);
        }

        private void BuildAllyBodies()
        {
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
                AllyList.Add(ally);
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
                        ally.tags |= AllyTag.Drone;
                        break;
                    case "MegaDroneBody":
                        ally.tags |= AllyTag.Drone | AllyTag.UseShield;
                        break;
                    case "DroneCommanderBody":
                    case "RoboBallGreenBuddyBody":
                    case "RoboBallRedBuddyBody":
                        ally.tags |= AllyTag.Drone | AllyTag.Item;
                        break;
                    case "BeetleGuardAllyBody":
                        ally.tags |= AllyTag.Item;
                        break;
                    case "Turret1Body":
                        ally.tags |= AllyTag.Drone | AllyTag.Turret | AllyTag.UseShield;
                        break;
                    case "SquidTurretBody":
                        ally.tags |= AllyTag.Item | AllyTag.Turret;
                        break;
                    case "MinorConstructAllyBody":
                        ally.tags |= AllyTag.Item | AllyTag.Turret | AllyTag.DontModifyRegen;
                        break;
                    default:
                        break;
                }
                AllyList.Add(ally);
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

    [Flags]
    public enum AllyTag : uint
    {
        None = 0u,
        Drone = 1u,  //Benefits from Droneman
        Item = 2u,   //Is an item effect
        Turret = 4u,  //Resistance to AOE/Proc
        DontModifyScaling = 8u,
        UseShield = 16u,
        DontModifyRegen = 32u
    }
}
