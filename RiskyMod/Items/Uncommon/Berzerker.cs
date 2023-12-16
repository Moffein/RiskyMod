using UnityEngine;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Uncommon
{
    public class Berzerker
    {
        public static bool enabled = true;
        public static BuffDef berzerkBuff;
        public Berzerker()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.AddMultiKill += (il) =>
             {
                 ILCursor c = new ILCursor(il);
                 if(c.TryGotoNext(
                      x => x.MatchLdsfld(typeof(RoR2Content.Items), "WarCryOnMultiKill")
                     ))
                 {
                     c.Remove();
                     c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                 }
                 else
                 {
                     UnityEngine.Debug.LogError("RiskyMod: Berzerker AddMultiKill IL Hook failed");
                 }
             };

            berzerkBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_BerzerkBuff",
                true,
                false,
                false,
                new Color(210f / 255f, 50f / 255f, 22f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/WarCryBuff").iconSprite
                );

            AssistManager.HandleAssistInventoryActions += OnKillEffect;
            RecalculateStatsAPI.GetStatCoefficients += HandleStats;

            IL.RoR2.CharacterBody.OnClientBuffsChanged += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "WarCryBuff")
                    ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasWarCry, self) =>
                    {
                        return hasWarCry || self.HasBuff(Berzerker.berzerkBuff);
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Berzerker OnClientBuffsChanged IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.WarCryOnMultiKill);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.WarCryOnMultiKill);
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int berzerkCount = sender.GetBuffCount(Berzerker.berzerkBuff);
            if (berzerkCount > 0)
            {
                args.moveSpeedMultAdd += 0.12f * berzerkCount;
                args.attackSpeedMultAdd += 0.12f * berzerkCount;
            }
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {

            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
            if (itemCount > 0)
            {
                int maxStacks = 1 + 2 * itemCount;
                int desiredStacks = Math.Min(attackerBody.GetBuffCount(Berzerker.berzerkBuff) + 1, maxStacks);

                attackerBody.ClearTimedBuffs(Berzerker.berzerkBuff);
                for (int i = 0; i < desiredStacks; i++)
                {
                    attackerBody.AddTimedBuff(Berzerker.berzerkBuff, 4f);
                }
            }
        }
    }
}
