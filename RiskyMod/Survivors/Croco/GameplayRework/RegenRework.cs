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
        public static float regenDuration = 2f;
        private static float regenAmount;
        //private static float regenReductionOnHit = 0f;    //was 0.36
        //private static float totalRegenReduction;

        public RegenRework()
        {
            regenAmount = 0.1f / regenDuration;
            //totalRegenReduction = regenDuration * regenReductionOnHit;

            BuffDef bd = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CrocoRegen");
            CrocoRegen2 = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_CrocoRegen",
                true,
                false,
                false,
                bd.buffColor,
               bd.iconSprite
                );

            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "CrocoRegen")
                    ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || self.HasBuff(CrocoRegen2);
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Croco RegenRework UpdateAllTemporaryVisualEffects IL Hook failed");
                }
            };

            //SharedHooks.OnHitEnemy.OnHitNoAttackerActions += DamageReducesRegen;
            RecalculateStatsAPI.GetStatCoefficients += HandleStats;

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

        /*private static void DamageReducesRegen(DamageInfo damageInfo, CharacterBody victimBody)
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
        }*/

        private void ReplaceM1Regen()
        {
            IL.EntityStates.Croco.Slash.OnMeleeHitAuthority += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                        x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "CrocoRegen")
                       ))
                {
                    c.Remove();
                    c.Emit<RegenRework>(OpCodes.Ldsfld, nameof(CrocoRegen2));
                    if(c.TryGotoNext(
                            x => x.MatchLdcR4(0.5f)
                           ))
                    {
                        c.Next.Operand = RegenRework.regenDuration;
                        error = false;
                    }
                }

                if(error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Croco RegenRework Slash IL Hook failed");
                }
            };
        }

        private void ReplaceBiteRegen()
        {
            IL.EntityStates.Croco.Bite.OnMeleeHitAuthority += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                        x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "CrocoRegen")
                       ))
                {
                    c.Remove();
                    c.Emit<RegenRework>(OpCodes.Ldsfld, nameof(CrocoRegen2));
                    if(c.TryGotoNext(
                            x => x.MatchLdcR4(0.5f)
                           ))
                    {
                        c.Next.Operand = RegenRework.regenDuration;
                        error = false;
                    }
                }
                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Croco RegenRework Bite IL Hook failed");
                }
            };
        }
    }
}
