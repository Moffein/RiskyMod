using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
namespace Risky_Mod.Items.Uncommon
{
    public class LeechingSeed
    {
        public static bool enabled = true;
        public static void Modify()
        {
			if (!enabled) return;

			//Remove Vanilla Effect
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
					 x => x.MatchLdsfld(typeof(RoR2Content.Items), "Seed")
					);
				c.Remove();
				c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
			};

			//Effect is handled in SharedHooks.OnHitEnemy

			LanguageAPI.Add("ITEM_SEED_DESC", "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>0.5% <style=cStack>(+0.5% per stack)</style> of your max health</style>.");
		}
    }
}
