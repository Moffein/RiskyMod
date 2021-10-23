using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
namespace RiskyMod.Items.Uncommon
{
    public class Guillotine
    {
        public static bool enabled = true;
		public static float damageCoefficient = 0.25f;

		public Guillotine()
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
				c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
			};

			//Effect is handled in SharedHooks.ModifyFinalDamage

			LanguageAPI.Add("ITEM_EXECUTELOWHEALTHELITE_PICKUP", "Deal bonus damage to enemies below 30% health.");
			LanguageAPI.Add("ITEM_EXECUTELOWHEALTHELITE_DESC", "Deal <style=cIsDamage>+"+ItemsCore.ToPercent(damageCoefficient)+"</style> <style=cStack>(+" + ItemsCore.ToPercent(damageCoefficient) + " per stack)</style> damage to enemies below <style=cIsDamage>30% health</style>.");
		}
	}
}
