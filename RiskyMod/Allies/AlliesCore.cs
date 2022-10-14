using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using RiskyMod.Allies.DroneChanges;
using R2API;

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

        public static ItemDef AllyRegenItem;
        public static ItemDef AllyMarkerItem;
        public static ItemDef AllyScalingItem;
        public static ItemDef AllyAllowVoidDeathItem;

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
            BuildAllyItems();

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
                    case "EquipmentDroneBody":
                    case "EmergencyDroneBody":
                        ally.tags |= AllyTag.Drone;
                        break;
                    case "FlameDroneBody":
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

        private void BuildAllyItems()
        {
            BuildAllyItem();
            BuildAllyScalingItem();
            BuildAllyAllowVoidDeathItem();
            BuildAllyRegenItem();
        }

        private void BuildAllyAllowVoidDeathItem()
        {
            if (AlliesCore.AllyAllowVoidDeathItem) return;
            AllyAllowVoidDeathItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyAllowVoidDeathItem.canRemove = false;
            AllyAllowVoidDeathItem.name = "RiskyModAllyAllowVoidDeathItem";
            AllyAllowVoidDeathItem.deprecatedTier = ItemTier.NoTier;
            AllyAllowVoidDeathItem.descriptionToken = "Allows this player-allied NPC to die to Void implosions.";
            AllyAllowVoidDeathItem.nameToken = "NPC Ally Allow Void Death";
            AllyAllowVoidDeathItem.pickupToken = "Allows this player-allied NPC to die to Void implosions.";
            AllyAllowVoidDeathItem.hidden = true;
            AllyAllowVoidDeathItem.pickupIconSprite = null;
            AllyAllowVoidDeathItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(AllyAllowVoidDeathItem, idr));
        }
        private void BuildAllyRegenItem()
        {
            if (AlliesCore.AllyRegenItem) return;
            AllyRegenItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyRegenItem.canRemove = false;
            AllyRegenItem.name = "RiskyModAllyRegenItem";
            AllyRegenItem.deprecatedTier = ItemTier.NoTier;
            AllyRegenItem.descriptionToken = "Regenerate to full HP in 1 (+1 per stack) second.";
            AllyRegenItem.nameToken = "NPC Ally Allow Void Death";
            AllyRegenItem.pickupToken = "Regenerate to full HP in 1 (+1 per stack) second.";
            AllyRegenItem.hidden = true;
            AllyRegenItem.pickupIconSprite = null;
            AllyRegenItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(AllyRegenItem, idr));

            if (AlliesCore.enabled) SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += AllyRegenItemDelegate;
        }

        private static void AllyRegenItemDelegate(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int itemCount = inventory.GetItemCount(AlliesCore.AllyRegenItem);
            if (itemCount > 0)
            {
                float levelFactor = sender.level - 1f;

                float targetRegen = (sender.baseMaxHealth + levelFactor * sender.levelMaxHealth) / itemCount;
                float currentRegen = sender.baseRegen + levelFactor * sender.levelRegen;
                args.baseRegenAdd += targetRegen - currentRegen;
            }
        }

        private void BuildAllyItem()
        {
            if (AlliesCore.AllyMarkerItem) return;

            AllyMarkerItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyMarkerItem.canRemove = false;
            AllyMarkerItem.name = "RiskyModAllyItem";
            AllyMarkerItem.deprecatedTier = ItemTier.NoTier;
            AllyMarkerItem.descriptionToken = "Gain the bonuses given to player-allied NPCs.";
            AllyMarkerItem.nameToken = "NPC Ally Marker";
            AllyMarkerItem.pickupToken = "Gain the bonuses given to player-allied NPCs.";
            AllyMarkerItem.hidden = true;
            AllyMarkerItem.pickupIconSprite = null;
            AllyMarkerItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(AllyMarkerItem, idr));

            if (AllyScaling.noOverheat || AllyScaling.noVoidDeath) SharedHooks.RecalculateStats.HandleRecalculateStatsInventoryActions += AllyMarkerItemDelegate;
        }

        private static void AllyMarkerItemDelegate (CharacterBody self, Inventory inventory)
        {
            if (inventory.GetItemCount(AlliesCore.AllyMarkerItem) > 0)
            {
                if (AllyScaling.noOverheat && !self.bodyFlags.HasFlag(CharacterBody.BodyFlags.OverheatImmune)) self.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;
                if (AllyScaling.noVoidDeath && !self.bodyFlags.HasFlag(CharacterBody.BodyFlags.ImmuneToVoidDeath) && inventory.GetItemCount(AlliesCore.AllyAllowVoidDeathItem) <= 0) self.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
            }
        }

        private void BuildAllyScalingItem()
        {
            if (AlliesCore.AllyScalingItem) return;

            AllyScalingItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyScalingItem.canRemove = false;
            AllyScalingItem.name = "RiskyModAllyScalingItem";
            AllyScalingItem.deprecatedTier = ItemTier.NoTier;
            AllyScalingItem.descriptionToken = "Swap HP and Damage scaling.";
            AllyScalingItem.nameToken = "NPC Ally Scaling";
            AllyScalingItem.pickupToken = "Swap HP and Damage scaling.";
            AllyScalingItem.hidden = true;
            AllyScalingItem.pickupIconSprite = null;
            AllyScalingItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(AllyScalingItem, idr));

            if (AlliesCore.enabled) SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += AllyScalingItemDelegate;
        }

        private static void AllyScalingItemDelegate(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (inventory.GetItemCount(AlliesCore.AllyScalingItem) > 0)
            {
                float levelFactor = sender.level - 1f;
                args.baseDamageAdd += 0.1f * levelFactor * sender.baseDamage;
                args.baseHealthAdd -= 0.1f * levelFactor * sender.baseMaxHealth;
            }
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
