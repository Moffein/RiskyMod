using R2API;

namespace RiskyMod.Items.Uncommon
{
    public class WarHorn
    {
        public static bool enabled = true;
        public WarHorn()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_ENERGIZEDONEQUIPMENTUSE_PICKUP", "Activating your Equipment gives you a burst of movement speed and attack speed.");
            LanguageAPI.Add("ITEM_ENERGIZEDONEQUIPMENTUSE_DESC",
                "Activating your Equipment gives you <style=cIsUtility>+50% movement speed</style> and <style=cIsDamage>+70% attack speed</style> for <style=cIsDamage>8s</style> <style=cStack>(+4s per stack)</style>.");

            //Buff handled in SharedHooks.GetStatCoefficients
        }
    }
}
