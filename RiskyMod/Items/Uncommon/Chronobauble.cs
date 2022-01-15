using R2API;
using RoR2;

namespace RiskyMod.Items.Uncommon
{
    public class Chronobauble
    {
        public static bool enabled = true;
        public Chronobauble()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.SlowOnHit);
            RecalculateStatsAPI.GetStatCoefficients += ChronobaubleDebuff;
        }

        private static void ChronobaubleDebuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.Slow50.buffIndex))
            {
                args.attackSpeedMultAdd -= 0.2f;
            }
        }
    }
}
