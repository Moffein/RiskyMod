using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
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

            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove Vanilla bleed effect - needs to be recalculated.
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Shatterspleen OnHitEnemy IL Hook failed");
                }
            };

            GetStatCoefficients.HandleStatsInventoryActions += AddBleedChance;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    ))
                {
                    //Change explosion damage
                    if(c.TryGotoNext(
                         x => x.MatchLdcR4(4f)
                        ))
                    {
                        c.Next.Operand = 3.2f;
                        c.Index += 8;
                        c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                        {
                            return damageCoefficient + 0.8f;
                        });

                        //Change Max HP damage
                        if (c.TryGotoNext(
                                 x => x.MatchLdcR4(0.15f)
                                ))
                        {
                            c.Next.Operand = 0.08f;
                            c.Index += 8;
                            c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                            {
                                return damageCoefficient + 0.02f;
                            });


                            //Disable Proc Coefficient
                            if (RiskyMod.disableProcChains)
                            {
                                if(c.TryGotoNext(
                                    x => x.MatchStfld<DelayBlast>("position")
                                    ))
                                {
                                    c.Index--;
                                    c.EmitDelegate<Func<DelayBlast, DelayBlast>>((db) =>
                                    {
                                        db.procCoefficient = 0f;
                                        return db;
                                    });
                                }
                            }

                            error = false;
                        }
                    }
                }
                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Shatterspleen OnCharacterDeath IL Hook failed");
                }
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.BleedOnHitAndExplode);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BleedOnHitAndExplode);
        }

        private static void AddBleedChance(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode) > 0)
            {
                sender.bleedChance += 5f;
            }
        }
    }
}
