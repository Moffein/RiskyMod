using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.Equipment
{
    public class CritHud
    {
        public static bool enabled = true;
        public CritHud()
        {
            if (!enabled) return;

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdarg(1),
                     x => x.MatchLdfld<DamageInfo>("crit")
                    );

                c.GotoNext(
                     x => x.MatchLdcR4(2f)
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_1);//damageinfo
                c.EmitDelegate<Func<float, DamageInfo, float>>((critMult, damageInfo) =>
                {
                    if (damageInfo.attacker)
                    {
                        CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                        if (attackerBody)
                        {
                            //Roll for another crit if attacker is using HUD
                            if (attackerBody.crit > 100f && attackerBody.HasBuff(RoR2Content.Buffs.FullCrit))
                            {
                                float trueCrit = attackerBody.crit - 100f;
                                if (trueCrit >= 100f || Util.CheckRoll(trueCrit, attackerBody.master))
                                {
                                    critMult *= 1.5f;
                                }
                            }
                        }
                    }
                    return critMult;
                });
            };
        }
    }
}
