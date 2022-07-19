using RoR2;
using R2API;

namespace RiskyMod.Items.DLC1.Void
{
    public class Needletick
    {
        public static bool enabled = true;

        public Needletick()
        {
            if (!enabled) return;

            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                int collapseCount = sender.GetBuffCount(DLC1Content.Buffs.Fracture);
                args.armorAdd -= collapseCount;
            };
        }
    }
}
