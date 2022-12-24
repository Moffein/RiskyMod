using RoR2;
using UnityEngine;
using R2API;

namespace RiskyMod.Allies
{
    public static class AllyItems
    {
        public static ItemDef AllyMarkerItem;
        public static ItemDef AllyScalingItem;
        public static ItemDef AllyRegenItem;
        public static ItemDef AllyAllowVoidDeathItem;
        public static ItemDef AllyAllowOverheatDeathItem;
        public static ItemDef AllyResistAoEItem;

        public static void Init()
        {
            BuildAllyMarkerItem();
            BuildAllyScalingItem();
            BuildAllyRegenItem();
            BuildAllyAllowVoidDeathItem();
            BuildAllyAllowOverheatDeathItem();
            BuildAllyResistAoEItem();
        }

        private static void BuildAllyResistAoEItem()
        {
            if (AllyItems.AllyResistAoEItem) return;
            AllyResistAoEItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyResistAoEItem.canRemove = false;
            AllyResistAoEItem.name = "RiskyModAllyResistAoEItem";
            AllyResistAoEItem.deprecatedTier = ItemTier.NoTier;
            AllyResistAoEItem.descriptionToken = "Gain +300 armor against AoE attacks.";
            AllyResistAoEItem.nameToken = "NPC Ally AoE Resist";
            AllyResistAoEItem.pickupToken = "Gain +300 armor against AoE attacks.";
            AllyResistAoEItem.hidden = true;
            AllyResistAoEItem.pickupIconSprite = null;
            AllyResistAoEItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(AllyResistAoEItem, idr));

            if (AlliesCore.enabled) SharedHooks.RecalculateStats.HandleRecalculateStatsInventoryActions += HandleAllyResistAoEItem;
        }

        private static void HandleAllyResistAoEItem(CharacterBody self, Inventory inventory)
        {
            if (!SoftDependencies.KingKombatArenaActive && inventory.GetItemCount(AllyItems.AllyResistAoEItem) > 0)
            {
                if (self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player)
                {
                    if (!self.bodyFlags.HasFlag(CharacterBody.BodyFlags.ResistantToAOE)) self.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                }
                else
                {
                    if (self.bodyFlags.HasFlag(CharacterBody.BodyFlags.ResistantToAOE)) self.bodyFlags &= ~CharacterBody.BodyFlags.ResistantToAOE;
                }
            }
        }

        //Effect is a part of AllyMarkerItem's delegate
        private static void BuildAllyAllowVoidDeathItem()
        {
            if (AllyItems.AllyAllowVoidDeathItem) return;
            AllyAllowVoidDeathItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyAllowVoidDeathItem.canRemove = false;
            AllyAllowVoidDeathItem.name = "RiskyModAllyAllowVoidDeathItem";
            AllyAllowVoidDeathItem.deprecatedTier = ItemTier.NoTier;
            AllyAllowVoidDeathItem.descriptionToken = "Allows this player-allied NPC to die to Void implosions.";
            AllyAllowVoidDeathItem.nameToken = "RiskyModAllyAllowVoidDeathItem";
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

        //Effect is a part of AllyMarkerItem's delegate
        private static void BuildAllyAllowOverheatDeathItem()
        {
            if (AllyItems.AllyAllowOverheatDeathItem) return;
            AllyAllowOverheatDeathItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyAllowOverheatDeathItem.canRemove = false;
            AllyAllowOverheatDeathItem.name = "RiskyModAllyAllowOverheatDeathItem";
            AllyAllowOverheatDeathItem.deprecatedTier = ItemTier.NoTier;
            AllyAllowOverheatDeathItem.descriptionToken = "Allows this player-allied NPC to die to Grandparent Suns.";
            AllyAllowOverheatDeathItem.nameToken = "RiskyModAllyAllowOverheatDeathItem";
            AllyAllowOverheatDeathItem.pickupToken = "Allows this player-allied NPC to die to Grandparent Suns.";
            AllyAllowOverheatDeathItem.hidden = true;
            AllyAllowOverheatDeathItem.pickupIconSprite = null;
            AllyAllowOverheatDeathItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.AIBlacklist,
                ItemTag.CannotCopy
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(AllyAllowOverheatDeathItem, idr));
        }

        private static void BuildAllyRegenItem()
        {
            if (AllyItems.AllyRegenItem) return;
            AllyRegenItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyRegenItem.canRemove = false;
            AllyRegenItem.name = "RiskyModAllyRegenItem";
            AllyRegenItem.deprecatedTier = ItemTier.NoTier;
            AllyRegenItem.descriptionToken = "Regenerate to full HP in 1 (+1 per stack) second.";
            AllyRegenItem.nameToken = "RiskyModAllyRegenItem";
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

            if (AlliesCore.enabled && AlliesCore.buffRegen) SharedHooks.RecalculateStats.HandleRecalculateStatsInventoryActions += AllyRegenItemDelegate;
        }

        private static void AllyRegenItemDelegate(CharacterBody sender, Inventory inventory)
        {
            int itemCount = inventory.GetItemCount(AllyItems.AllyRegenItem);
            if (!SoftDependencies.KingKombatArenaActive && itemCount > 0)
            {
                float targetRegen = sender.maxHealth / itemCount;
                if (sender.regen < targetRegen) sender.regen = targetRegen;
            }
        }

        private static void BuildAllyMarkerItem()
        {
            if (AllyItems.AllyMarkerItem) return;

            AllyMarkerItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyMarkerItem.canRemove = false;
            AllyMarkerItem.name = "RiskyModAllyMarkerItem";
            AllyMarkerItem.deprecatedTier = ItemTier.NoTier;
            AllyMarkerItem.descriptionToken = "Gain the bonuses given to player-allied NPCs.";
            AllyMarkerItem.nameToken = "RiskyModAllyMarkerItem";
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

        //Don't try to add player team handling here due to certain enemies naturally having these flags.
        private static void AllyMarkerItemDelegate(CharacterBody self, Inventory inventory)
        {
            if (!SoftDependencies.KingKombatArenaActive && inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0)
            {
                if (AllyScaling.noOverheat && !self.bodyFlags.HasFlag(CharacterBody.BodyFlags.OverheatImmune) && inventory.GetItemCount(AllyItems.AllyAllowOverheatDeathItem) <= 0) self.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;
                if (AllyScaling.noVoidDeath && !self.bodyFlags.HasFlag(CharacterBody.BodyFlags.ImmuneToVoidDeath) && inventory.GetItemCount(AllyItems.AllyAllowVoidDeathItem) <= 0) self.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
            }
        }

        private static void BuildAllyScalingItem()
        {
            if (AllyItems.AllyScalingItem) return;

            AllyScalingItem = ScriptableObject.CreateInstance<ItemDef>();
            AllyScalingItem.canRemove = false;
            AllyScalingItem.name = "RiskyModAllyScalingItem";
            AllyScalingItem.deprecatedTier = ItemTier.NoTier;
            AllyScalingItem.descriptionToken = "Swap HP and Damage scaling.";
            AllyScalingItem.nameToken = "RiskyModAllyScalingItem";
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

            if (AlliesCore.enabled && AlliesCore.changeScaling) SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += AllyScalingItemDelegate;
        }

        private static void AllyScalingItemDelegate(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (!SoftDependencies.KingKombatArenaActive && inventory.GetItemCount(AllyItems.AllyScalingItem) > 0 && sender.teamComponent && sender.teamComponent.teamIndex == TeamIndex.Player)
            {
                float levelFactor = sender.level - 1f;
                args.baseDamageAdd += 0.1f * levelFactor * sender.baseDamage;
                args.baseHealthAdd -= 0.1f * levelFactor * sender.baseMaxHealth;

                if (sender.levelMaxShield == sender.baseMaxShield * 0.3f)
                {
                    args.baseShieldAdd -= 0.1f * levelFactor * sender.baseMaxShield;
                }
            }
        }
    }
}
