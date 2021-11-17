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
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.EnergizedOnEquipmentUse);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.EnergizedOnEquipmentUse);

            //LanguageAPI.Add("ITEM_ENERGIZEDONEQUIPMENTUSE_PICKUP", "Activating your Equipment gives you a burst of movement speed and attack speed.");
            //LanguageAPI.Add("ITEM_ENERGIZEDONEQUIPMENTUSE_DESC",
            //    "Activating your Equipment gives you <style=cIsUtility>+50% movement speed</style> and <style=cIsDamage>+70% attack speed</style> for <style=cIsDamage>8s</style> <style=cStack>(+4s per stack)</style>.");

            GetStatsCoefficient.HandleStatsActions += HandleStats;
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
