﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
namespace RiskyMod.Items.Common
{
    public class StunGrenade
    {
        public static bool enabled = true;
        public StunGrenade()
        {
            if (!enabled) return;

            IL.RoR2.SetStateOnHurt.OnTakeDamageServer += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(MoveType.After,
                     x => x.MatchCall(typeof(RoR2.Util), "ConvertAmplificationPercentageIntoReductionPercentage")
                    ))
                {
                    c.Emit(OpCodes.Ldloc_3);    //ItemCount
                    c.Emit(OpCodes.Ldarg_1);    //DamageReport
                    c.EmitDelegate<Func<float, int, DamageReport, float>>((origChance, itemCount, damageReport) =>
                    {
                        return itemCount * SetStateOnHurt.stunChanceOnHitBaseChancePercent * damageReport.damageInfo.procCoefficient;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: StunGrenade IL Hook failed");
                }
            };
        }
    }
}
