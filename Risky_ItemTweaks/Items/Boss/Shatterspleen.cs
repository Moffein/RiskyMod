using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace Risky_Mod.Items.Boss
{
    public class Shatterspleen
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_BLEEDONHITANDEXPLODE_PICKUP", "Bleeding enemies explode on kill.");
            LanguageAPI.Add("ITEM_BLEEDONHITANDEXPLODE_DESC", "Gain a <style=cIsDamage>5%</style> chance to <style=cIsDamage>bleed</style> enemies for <style=cIsDamage>240%</style> base damage. <style=cIsDamage>Bleeding</style> enemies <style=cIsDamage>explode</style> on death for <style=cIsDamage>400%</style> <style=cStack>(+400% per stack)</style> damage, plus an additional <style=cIsDamage>10%</style> <style=cStack>(+10% per stack)</style> of their maximum health.");

            //Remove Vanilla bleed effect - needs to be recalculated.
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                //Add shatterspleen to bleed chance counter.
                ILCursor c = new ILCursor(il);

                //Having a Shatterspleen now triggers the bleed chance calculation by adding +1 tri-tip
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHit")
                    );
                c.GotoNext(
                    x => x.MatchCgt()
                    );
                c.Index++;
                c.Emit(OpCodes.Ldloc_3);    //inventory
                c.EmitDelegate<Func<bool, Inventory, bool>>((isBleed, inventory) =>
                {
                    if (!isBleed)
                    {
                        if (inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode) > 0)
                        {
                            isBleed = true;
                        }
                    }
                    return isBleed;
                });

                //Recalculate bleed chance
                c.GotoNext(
                     x =>x.MatchLdfld<DamageInfo>("procCoefficient")
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldloc_3);    //inventory
                c.Emit(OpCodes.Ldarg_1);    //damageinfo
                c.EmitDelegate<Func<float, Inventory, DamageInfo, float>>((origChance, inventory, damageInfo) =>
                {
                    if (inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode) > 0)
                    {
                        origChance += 5f * damageInfo.procCoefficient;
                    }
                    return origChance;
                });


                //Remove vanilla bleed on crit
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    );
                c.Remove();
                c.Emit<Risky_Mod>(OpCodes.Ldsfld, nameof(Risky_Mod.emptyItemDef));
            };

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    );

                //Change Max HP damage
                c.GotoNext(
                     x => x.MatchLdcR4(0.15f)
                    );
                c.Next.Operand = 0.1f;

                //Disable Proc Coefficient
                if (Risky_Mod.disableProcChains)
                {
                    c.GotoNext(
                        x => x.MatchStfld<DelayBlast>("position")
                        );
                    c.Index--;
                    c.EmitDelegate<Func<DelayBlast, DelayBlast>>((db) =>
                    {
                        db.procCoefficient = 0f;
                        return db;
                    });
                }
            };
        }
    }
}
