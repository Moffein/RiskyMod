using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
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
        private static float regenReductionOnHit;

        public RegenRework()
        {
            regenAmount = 0.1f / regenDuration;
            regenReductionOnHit = regenDuration * 0.3f;

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

            On.RoR2.HealthComponent.ServerFixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.alive)
                {
                    int buffCount = self.body.GetBuffCount(CrocoRegen2.buffIndex);
                    if (buffCount > 0)
                    {
                        self.HealFraction(Time.fixedDeltaTime * buffCount * regenAmount, default(ProcChainMask));
                    }
                }
            };

            SharedHooks.OnHitEnemy.OnHitNoAttackerActions += DamageReducesRegen;

            ReplaceM1Regen();
            ReplaceBiteRegen();
        }

        private static void DamageReducesRegen(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (victimBody.HasBuff(CrocoRegen2))
            {
                float toRemove = damageInfo.procCoefficient * regenReductionOnHit;
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
