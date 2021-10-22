using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;

namespace Risky_ItemTweaks.Items.Uncommon
{
    public class WillOWisp
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "ExplodeOnDeath")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Effect handled in SharedHooks.OnCharacterDeath

            LanguageAPI.Add("ITEM_EXPLODEONDEATH_DESC", "On killing an enemy, spawn a <style=cIsDamage>lava pillar</style> in a <style=cIsDamage>16m</style> radius for <style=cIsDamage>350%</style> <style=cStack>(+280% per stack)</style> base damage.");
        }
    }
}
