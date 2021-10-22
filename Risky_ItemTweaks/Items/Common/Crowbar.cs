using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Risky_ItemTweaks.Items.Common
{
    public class Crowbar
    {
        public static bool enabled = true;

        public static void Modify()
        {
            if (!enabled) return;

            //Remove vanilla effect
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Crowbar")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            LanguageAPI.Add("ITEM_CROWBAR_DESC", "Deal <style=cIsDamage>+50%</style> <style=cStack>(+50% per stack)</style> damage to enemies above <style=cIsDamage>90% health</style>.");

            //Effect handled in SharedHooks.TakeDamage
        }
    }
}
