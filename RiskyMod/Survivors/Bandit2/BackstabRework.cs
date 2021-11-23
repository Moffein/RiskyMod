using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BackstabRework
    {
        public static bool enabled = true;
        public static DamageAPI.ModdedDamageType AlwaysBackstab;
        public static DamageAPI.ModdedDamageType FakeCrit;

        public BackstabRework()
        {
            if (!enabled) return;
            AlwaysBackstab = DamageAPI.ReserveDamageType();
            FakeCrit = DamageAPI.ReserveDamageType();

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);

                //Change Backstab Conditions
                c.GotoNext(
                     x => x.MatchCallvirt<CharacterBody>("get_canPerformBackstab")
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);    //healthcomponent
                c.Emit(OpCodes.Ldloc_1);    //attackerBody
                c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                c.Emit(OpCodes.Ldloc_3);    //vector
                c.EmitDelegate<Func<bool, HealthComponent, CharacterBody, DamageInfo, Vector3, bool>>((hasBackstabPassive, self, attackerBody, damageInfo, damageVector) =>
                {
                    return (hasBackstabPassive || DamageAPI.HasModdedDamageType(damageInfo, AlwaysBackstab))   //Character can backstab
                    && (damageInfo.damageType & DamageType.DoT) != DamageType.DoT   //Damage is not DoT
                    && (damageInfo.procChainMask.HasProc(ProcType.Backstab) || BackstabManager.IsBackstab(-damageVector, self.body));   //Character is actually performing a backstab
                });

                //Mark damageInfo as fake crit
                c.GotoNext(
                     x => x.MatchStfld<DamageInfo>("crit")
                    );
                c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                c.EmitDelegate<Func<bool, DamageInfo, bool>>((crit, damageInfo) =>
                {
                    if (!damageInfo.crit)
                    {
                        DamageAPI.AddModdedDamageType(damageInfo, FakeCrit);
                        damageInfo.procChainMask.AddProc(ProcType.AACannon);    //Weird stuff will happen if other mods use this. Need modded Proc Chains.
                    }
                    return crit;
                });

                //Change crit multiplier
                c.GotoNext(
                x => x.MatchLdfld<DamageInfo>("crit")
                    );
                c.GotoNext(
                x => x.MatchLdcR4(2f)
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<float, DamageInfo, float>>((critMult, damageInfo) =>
                {
                    if (damageInfo.procChainMask.HasProc(ProcType.Backstab))
                    {
                        if (damageInfo.HasModdedDamageType(FakeCrit) || damageInfo.procChainMask.HasProc(ProcType.AACannon))
                        {
                            critMult *= 0.75f;
                        }
                        else
                        {
                            critMult *= 1.5f;
                        }
                    }
                    return critMult;
                });
            };
        }
    }
}
