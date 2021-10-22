using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;

namespace Risky_ItemTweaks.Items.Common
{
    public class RepArmor
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove vanilla effect
            On.RoR2.HealthComponent.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                self.itemCounts.armorPlate = 0;
            };

            LanguageAPI.Add("ITEM_REPULSIONARMORPLATE_PICKUP", "Increase armor by 5.");
            LanguageAPI.Add("ITEM_REPULSIONARMORPLATE_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>5</style>.");

            //Effect handled on SharedHooks.GetStatCoefficients
        }
    }
}
