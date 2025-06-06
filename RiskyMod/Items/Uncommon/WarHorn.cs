﻿using R2API;
using RiskyMod.SharedHooks;
using RoR2;

namespace RiskyMod.Items.Uncommon
{
    public class WarHorn
    {
        public static bool enabled = true;
        public WarHorn()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            RecalculateStatsAPI.GetStatCoefficients += HandleStats;
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.EnergizedOnEquipmentUse);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.EnergizedOnEquipmentUse);
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {

            if (sender.HasBuff(RoR2Content.Buffs.Energized))
            {
                args.moveSpeedMultAdd += 0.5f;
            }
        }
    }
}
