using R2API;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.Legendary
{
    public class Headstompers
    {
        public static bool enabled = true;
        public Headstompers()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.FallBoots);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.FallBoots);

            On.EntityStates.Headstompers.BaseHeadstompersState.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.bodyMotor) self.bodyMotor.airControl = 1f; //Shouldnt this be adding to the bodyMotor, instead of assigning the value? Just wondering
            };

            On.EntityStates.Headstompers.BaseHeadstompersState.OnExit += (orig, self) =>
            {
                if (self.bodyMotor) self.bodyMotor.airControl = 0.25f; //Same here
                orig(self);
            };

            //LanguageAPI.Add("ITEM_FALLBOOTS_PICKUP", "Increase jump height and air control. Hold 'Interact' to slam down to the ground.");
            //LanguageAPI.Add("ITEM_FALLBOOTS_DESC", "Increase <style=cIsUtility>jump height</style> and <style=cIsUtility>air control</style>. Creates a <style=cIsDamage>5m-100m</style> radius <style=cIsDamage>kinetic explosion</style> on hitting the ground, dealing <style=cIsDamage>1000%-10000%</style> base damage that scales up with <style=cIsDamage>fall distance</style>. Recharges in <style=cIsDamage>10</style> <style=cStack>(-50% per stack)</style> seconds.");
        }
    }
}
