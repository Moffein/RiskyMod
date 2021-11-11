using R2API;
using RiskyMod.SharedHooks;
using RoR2;

namespace RiskyMod.Items.Uncommon
{
    public class Chronobauble
    {
        public static bool enabled = true;
        public Chronobauble()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_SLOWONHIT_PICKUP", "Impair enemies on hit.");
            LanguageAPI.Add("ITEM_SLOWONHIT_DESC", "<style=cIsUtility>Impair</style> enemies on hit for <style=cIsUtility>-60% movement speed</style>, <style=cIsDamage>-30% damage</style>, and <style=cIsDamage>-20 armor</style> for <style=cIsUtility>2s</style> <style=cStack>(+2s per stack)</style>.");

            GetStatsCoefficient.HandleStatsActions += HandleStats;
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.Slow60))
            {
                //Slow -60% already present in vanilla
                args.damageMultAdd -= 0.3f; //Might need to check for negative damage mult, come back to this later if there's problems.
                args.armorAdd -= 20f;
            }
        }
    }
}
