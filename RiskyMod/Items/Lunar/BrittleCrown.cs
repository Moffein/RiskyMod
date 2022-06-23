using RoR2;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace RiskyMod.Items.Lunar
{
    public class BrittleCrown
    {
        public static bool enabled = true;
        public BrittleCrown()
        {
            if (!enabled) return;

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "GoldOnHit")
                    )
                    &&
                c.TryGotoNext(
                     x => x.MatchLdcR4(30f)
                    ))
                {
                    c.Index++;
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<Func<float, DamageInfo, float>>((chance, damageInfo) =>
                    {
                        if (damageInfo.attacker)
                        {
                            CharacterBody cb = damageInfo.attacker.GetComponent<CharacterBody>();
                            if (cb)
                            {
                                float damageCoefficient = damageInfo.damage / cb.damage;
                                if (damageCoefficient > 1f)
                                {
                                    chance += Mathf.Lerp(0f, 70f, (damageCoefficient - 1f) * 0.111111111f);//Caps out at 100% chance for 1000% damage attacks
                                }
                            }
                        }
                        return chance;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: BrittleCrown IL Hook failed");
                }
            };
        }
    }
}
