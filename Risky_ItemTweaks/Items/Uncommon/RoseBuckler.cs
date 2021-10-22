using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;

namespace Risky_Mod.Items.Uncommon
{
    public class RoseBuckler
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "SprintArmor")
                    );
                c.Remove();
                c.Emit<Risky_Mod>(OpCodes.Ldsfld, nameof(Risky_Mod.emptyItemDef));
            };

            LanguageAPI.Add("ITEM_SPRINTARMOR_PICKUP", "Increase armor. Gain bonus armor while sprinting.");
            LanguageAPI.Add("ITEM_SPRINTARMOR_DESC", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>12</style> <style=cStack>(+12 per stack)</style>. <style=cIsUtility>Sprinting</style> increases TOTAL armor by <style=cIsHealing>+50%</style>.");

            //Effect handled on SharedHooks.GetStatCoefficients and SharedHooks.RecalculateStats
        }
    }
}
