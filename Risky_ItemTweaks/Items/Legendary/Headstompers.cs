using R2API;
using RoR2;
using UnityEngine;

namespace Risky_Mod.Items.Legendary
{
    public class Headstompers
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;
            On.EntityStates.Headstompers.BaseHeadstompersState.OnEnter += (orig, self) =>
            {
                orig(self);
                self.bodyMotor.airControl = 1f;
            };

            On.EntityStates.Headstompers.BaseHeadstompersState.OnExit += (orig, self) =>
            {
                self.bodyMotor.airControl = 0.25f;
                orig(self);
            };

            LanguageAPI.Add("ITEM_FALLBOOTS_PICKUP", "Increase jump height and air control. Hold 'Interact' to slam down to the ground.");
            LanguageAPI.Add("ITEM_FALLBOOTS_DESC", "Increase <style=cIsUtility>jump height</style> and <style=cIsUtility>air control</style>. Creates a <style=cIsDamage>5m-100m</style> radius <style=cIsDamage>kinetic explosion</style> on hitting the ground, dealing <style=cIsDamage>1000%-10000%</style> base damage that scales up with <style=cIsDamage>fall distance</style>. Recharges in <style=cIsDamage>10</style> <style=cStack>(-50% per stack)</style> seconds.");
        }
    }
}
