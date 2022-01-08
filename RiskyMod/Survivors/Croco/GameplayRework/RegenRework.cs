using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Croco
{
    public class RegenRework
    {
        public static BuffDef CrocoRegen2;
        public static float regenDuration = 1.5f;
        private static float regenAmount;
        private static float regenReductionOnHit = 0.36f;
        private static float totalRegenReduction;

        public RegenRework()
        {
            regenAmount = 0.1f / regenDuration;
            totalRegenReduction = regenDuration * regenReductionOnHit;

            CrocoRegen2 = ScriptableObject.CreateInstance<BuffDef>();
            CrocoRegen2.buffColor = RoR2Content.Buffs.CrocoRegen.buffColor;
            CrocoRegen2.canStack = true;
            CrocoRegen2.isDebuff = false;
            CrocoRegen2.name = "RiskyMod_CrocoRegen";
            CrocoRegen2.iconSprite = RoR2Content.Buffs.CrocoRegen.iconSprite;
            BuffAPI.Add(new CustomBuff(CrocoRegen2));

            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "CrocoRegen")
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
                {
                    return hasBuff || self.HasBuff(CrocoRegen2);
                });
            };

            SharedHooks.OnHitEnemy.OnHitNoAttackerActions += DamageReducesRegen;
            GetStatsCoefficient.HandleStatsActions += HandleStats;

            ReplaceM1Regen();
            ReplaceBiteRegen();
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffCount = sender.GetBuffCount(CrocoRegen2.buffIndex);
            if (buffCount > 0)
            {
                args.baseRegenAdd += buffCount * (sender.maxHealth * regenAmount);
            }
        }

        private static void DamageReducesRegen(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (victimBody.HasBuff(CrocoRegen2))
            {
                float toRemove = damageInfo.procCoefficient * totalRegenReduction;
                foreach (CharacterBody.TimedBuff tb in victimBody.timedBuffs)
                {
                    if (tb.buffIndex == CrocoRegen2.buffIndex)
                    {
                        tb.timer -= toRemove;
                    }
                }
            }
        }

        private void ReplaceM1Regen()
        {
            IL.EntityStates.Croco.Slash.OnMeleeHitAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                        x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "CrocoRegen")
                       );
                c.Remove();
                c.Emit<RegenRework>(OpCodes.Ldsfld, nameof(CrocoRegen2));
                c.GotoNext(
                        x => x.MatchLdcR4(0.5f)
                       );
                c.Next.Operand = RegenRework.regenDuration;
            };
        }

        private void ReplaceBiteRegen()
        {
            IL.EntityStates.Croco.Bite.OnMeleeHitAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                        x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "CrocoRegen")
                       );
                c.Remove();
                c.Emit<RegenRework>(OpCodes.Ldsfld, nameof(CrocoRegen2));
                c.GotoNext(
                        x => x.MatchLdcR4(0.5f)
                       );
                c.Next.Operand = RegenRework.regenDuration;
            };
        }
    }
}
