using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
namespace Risky_Mod.Items.Uncommon
{
    public class Guillotine
    {
        public static bool enabled = true;
		public static void Modify()
		{
			if (!enabled) return;

			//Remove Vanilla Effect
			IL.RoR2.CharacterBody.OnInventoryChanged += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "ExecuteLowHealthElite")
					);
				c.Remove();
				c.Emit<Risky_Mod>(OpCodes.Ldsfld, nameof(Risky_Mod.emptyItemDef));
			};

			//Effect is handled in SharedHooks.ModifyFinalDamage

			LanguageAPI.Add("ITEM_EXECUTELOWHEALTHELITE_PICKUP", "Deal bonus damage to enemies below 30% health.");
			LanguageAPI.Add("ITEM_EXECUTELOWHEALTHELITE_DESC", "Deal <style=cIsDamage>+25%</style> <style=cStack>(+25% per stack)</style> damage to enemies below <style=cIsDamage>30% health</style>.");
		}
	}
}
