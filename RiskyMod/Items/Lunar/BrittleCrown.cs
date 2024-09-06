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

        public static float minDamageCoefficient = 2f;
        public static float maxDamageCoefficient = 12f;

        public BrittleCrown()
        {
            if (!enabled) return;

            IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
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
                                float damageDiff = damageCoefficient - BrittleCrown.minDamageCoefficient;
                                if (damageDiff > 0f)
                                {
                                    //70f due to 30% base chance
                                    chance += Mathf.Lerp(0f, 70f, damageDiff / (BrittleCrown.maxDamageCoefficient - BrittleCrown.minDamageCoefficient));
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
