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
            On.RoR2.ItemCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.BleedOnHitAndExplode);
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BleedOnHitAndExplode);
            };

            //Remove Vanilla bleed effect - needs to be recalculated.
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BleedOnHitAndExplode")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            //TODO: Modify args instead once bleed chance gets added there
            //GetStatsCoefficient.HandleStatsInventoryActions += AddBleedChance;
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.inventory && self.inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode) > 0)
                {
                    self.bleedChance += 5f;
                }
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

        private static void AddBleedChance(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode) > 0)
            {
                sender.bleedChance += 5f;
            }
        }
    }
}
