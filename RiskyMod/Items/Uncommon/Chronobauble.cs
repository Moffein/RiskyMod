﻿using R2API;
using RoR2;

namespace RiskyMod.Items.Uncommon
{
    public class Chronobauble
    {
        public static bool enabled = true;

        private const float attackSpeedReductionFactor = (1f / 0.8f) - 1f;

        public Chronobauble()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            RecalculateStatsAPI.GetStatCoefficients += ChronobaubleDebuff;
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.SlowOnHit);
        }

        private static void ChronobaubleDebuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.Slow60.buffIndex))
            {
                args.attackSpeedReductionMultAdd += attackSpeedReductionFactor;
                args.moveSpeedReductionMultAdd += 0.2f; //0.6f vanilla is included, goes to 0.8f total
                args.cooldownMultAdd += 0.2f;
            }
        }
    }
}
