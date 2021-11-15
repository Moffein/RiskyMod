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
            LanguageAPI.Add("ITEM_SLOWONHIT_DESC", "<style=cIsUtility>Impair</style> enemies on hit, reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>-60%</style> and increasing <style=cIsDamage>damage taken</style> by <style=cIsDamage>15%</style> for <style=cIsUtility>2s</style> <style=cStack>(+2s per stack)</style>.");
        }

        //Handled in ModifyFinalDamage
    }
}
