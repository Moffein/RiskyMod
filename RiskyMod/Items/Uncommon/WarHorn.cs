using R2API;
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
            On.RoR2.ItemCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.EnergizedOnEquipmentUse);
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.EnergizedOnEquipmentUse);
            };
            RecalculateStatsAPI.GetStatCoefficients += HandleStats;
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
