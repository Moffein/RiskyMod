using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
namespace RiskyMod.Items.Common
{
    public class StunGrenade
    {
        public static bool enabled = true;
        public StunGrenade()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.SetStateOnHurt.OnTakeDamageServer += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), "StunChanceOnHit")) && c.TryGotoNext(MoveType.After,
                     x => x.MatchCall(typeof(RoR2.Util), "ConvertAmplificationPercentageIntoReductionPercentage")
                    ))
                {
                    c.Emit(OpCodes.Ldloc_3);    //ItemCount
                    c.Emit(OpCodes.Ldarg_1);    //DamageReport
                    c.EmitDelegate<Func<float, int, DamageReport, float>>((origChance, itemCount, damageReport) =>
                    {
                        return itemCount * SetStateOnHurt.stunChanceOnHitBaseChancePercent * damageReport.damageInfo.procCoefficient;
                    });

                    if (c.TryGotoNext(x => x.MatchCall<SetStateOnHurt>("SetStun")))
                    {
                        c.Emit(OpCodes.Ldarg_1);
                        c.EmitDelegate<Func<float, DamageReport, float>>((origStunDuration, damageReport) =>
                        {
                            if (damageReport.victimBody && damageReport.victimBody.healthComponent)
                            {
                                DamageInfo damageInfo = damageReport.damageInfo;
                                damageReport.victimBody.healthComponent.TakeDamage(new DamageInfo()
                                {
                                    damage = damageInfo.damage * 0.5f,
                                    attacker = damageInfo.attacker,
                                    canRejectForce = true,
                                    crit = damageInfo.crit,
                                    damageType = DamageType.Generic,
                                    damageColorIndex = DamageColorIndex.Item,
                                    force = Vector3.zero,
                                    inflictor = damageInfo.attacker,
                                    procCoefficient = 0f,
                                    procChainMask = damageInfo.procChainMask,
                                    position = damageInfo.position,
                                    dotIndex = DotController.DotIndex.None,
                                    inflictedHurtbox = damageInfo.inflictedHurtbox
                                });
                            }
                            return origStunDuration;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: StunGrenade IL Hook failed");
                }
            };
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.StunChanceOnHit);
        }
    }
}
