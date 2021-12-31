using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace RiskyMod.Items.Boss
{
    public class Shatterspleen
    {
        public static bool enabled = true;
        public Shatterspleen()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.BleedOnHitAndExplode);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BleedOnHitAndExplode);

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
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    );

                //Change explosion damage
                c.GotoNext(
                     x => x.MatchLdcR4(4f)
                    );
                c.Next.Operand = 3.2f;
                c.Index += 8;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + 0.8f;
                });

                //Change Max HP damage
                c.GotoNext(
                     x => x.MatchLdcR4(0.15f)
                    );
                c.Next.Operand = 0.08f;
                c.Index += 8;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + 0.02f;
                });


                //Disable Proc Coefficient
                if (RiskyMod.disableProcChains)
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

                //Disable falloff
                /*c.GotoNext(
                    x => x.MatchStfld<DelayBlast>("falloffModel")
                    );

                c.EmitDelegate<Func<BlastAttack.FalloffModel, BlastAttack.FalloffModel>>((model) =>
                {
                    return BlastAttack.FalloffModel.None;
                });*/
            };
        }
    }
}
