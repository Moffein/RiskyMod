using RiskyMod.SharedHooks;
using RoR2;
using R2API;
using EntityStates.RiskyMod.Bandit2.Revolver;
using MonoMod.Cil;
using System;
using Mono.Cecil.Cil;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class SpecialDamageTweaks
    {
        public static bool specialExecute = true;
        public static float specialExecuteFraction = 0.1f;

        public SpecialDamageTweaks()
        {
            OnHitEnemy.OnHitNoAttackerActions += ApplyDebuff;
            TakeDamage.ModifyInitialDamageActions += RackEmUpBonus;
            if (specialExecute)
            {
                IL.RoR2.HealthComponent.TakeDamage += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchCallvirt<CharacterBody>("get_executeEliteHealthFraction")
                        );

                    c.GotoNext(
                         x => x.MatchLdloc(40),
                         x => x.MatchLdcR4(0.0f)
                        );
                    c.Index++;
                    c.Emit(OpCodes.Ldarg_0);    //healthcomponent
                    c.Emit(OpCodes.Ldarg_1);    //damageInfo
                    c.EmitDelegate <Func<float, HealthComponent, DamageInfo, float>> ((executeFraction, self, damageInfo) =>
                    {
                        if (executeFraction < 0f)
                        {
                            executeFraction = 0f;
                        }
                        if ((self.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToExecutes) <= CharacterBody.BodyFlags.None)
                        {
                            if (damageInfo.HasModdedDamageType(Bandit2Core.SpecialDamage) || self.body.HasBuff(Bandit2Core.SpecialDebuff))
                            {
                                executeFraction += specialExecuteFraction;
                            }
                        }
                        return executeFraction;
                    });
                    c.Emit(OpCodes.Stloc, 40);
                    c.Emit(OpCodes.Ldloc, 40);
                };
            }
        }

        private static void RackEmUpBonus(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.RackEmUpDamage))
            {
                float mult = 1f + self.body.GetBuffCount(Bandit2Core.SpecialDebuff) * (FireRackEmUp.bonusDamageCoefficient / FireRackEmUp.damageCoefficient);
                damageInfo.damage *= mult;
            }
        }

        private static void ApplyDebuff(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.SpecialDamage))
            {
                float buffDuration = BanditSpecialGracePeriod.enabled ? BanditSpecialGracePeriod.duration : 1.2f;
                int specialCount = victimBody.GetBuffCount(Bandit2Core.SpecialDebuff) + 1;
                victimBody.ClearTimedBuffs(Bandit2Core.SpecialDebuff);
                for (int i = 0; i < specialCount; i++)
                {
                    victimBody.AddTimedBuff(Bandit2Core.SpecialDebuff, buffDuration);
                }
            }
        }
    }
}
