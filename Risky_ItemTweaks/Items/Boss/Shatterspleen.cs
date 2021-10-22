using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;

namespace Risky_ItemTweaks.Items.Boss
{
    public class Shatterspleen
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_BLEEDONHITANDEXPLODE_PICKUP", "Bleeding enemies explode on kill.");
            LanguageAPI.Add("ITEM_BLEEDONHITANDEXPLODE_DESC", "Gain a <style=cIsDamage>5%</style> chance to <style=cIsDamage>bleed</style> enemies for <style=cIsDamage>240%</style> base damage. <style=cIsDamage>Bleeding</style> enemies <style=cIsDamage>explode</style> on death for <style=cIsDamage>400%</style> <style=cStack>(+400% per stack)</style> damage, plus an additional <style=cIsDamage>10%</style> <style=cStack>(+10% per stack)</style> of their maximum health.");

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHit")    //Overwrite vanilla bleed chance so that Shatterspleen can be added to the tally.
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Effects handled in Sharedhooks.OnHitEnemy and SharedHooks.OnCharacterDeath
        }
    }
}
