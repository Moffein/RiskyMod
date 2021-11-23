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

        public BackstabRework()
        {
            if (!enabled) return;
            AlwaysBackstab = DamageAPI.ReserveDamageType();

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                //Backstabs trigger OnCrit even if DamageInfo is not crit
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdfld<DamageInfo>("crit")
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                c.EmitDelegate<Func<bool, DamageInfo, bool>>((crit, damageInfo) =>
                {
                    return crit || damageInfo.procChainMask.HasProc(ProcType.Backstab);
                });

                //Remove vanilla SuperBleed
                c.GotoNext(
                    x => x.MatchLdfld<DamageInfo>("damageType"),
                    x => x.MatchLdcI4(134217728)
                   );
                c.Index++;
                c.Next.Operand = 0;
            };

            //Redo SuperBleed effect
            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) =>
            {
                orig(self, damageInfo, victim);
                if ((damageInfo.crit || (!damageInfo.crit && damageInfo.procChainMask.HasProc(ProcType.Backstab)))
                && (damageInfo.damageType & DamageType.SuperBleedOnCrit) != DamageType.Generic)
                {
                    DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.SuperBleed, 15f * damageInfo.procCoefficient, 1f);
                }
            };

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

                //Remove Backstab Crit
                c.GotoNext(
                     x => x.MatchStfld<DamageInfo>("crit")
                    );
                c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                c.EmitDelegate<Func<bool, DamageInfo, bool>>((crit, damageInfo) =>
                {
                    return damageInfo.crit;
                });

                //Change crit multiplier
                c.GotoNext(
                x => x.MatchLdfld<DamageInfo>("crit")
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                c.EmitDelegate<Func<bool, DamageInfo, bool>>((crit, damageInfo) =>
                {
                    return crit || damageInfo.procChainMask.HasProc(ProcType.Backstab);
                });
                c.GotoNext(
                x => x.MatchLdcR4(2f)
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<float, DamageInfo, float>>((critMult, damageInfo) =>
                {
                    critMult = 1f;
                    if (damageInfo.crit)
                    {
                        critMult *= 2f;
                    }
                    if (damageInfo.procChainMask.HasProc(ProcType.Backstab))
                    {
                        critMult *= 1.5f;
                        damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                    }
                    return critMult;
                });
            };
        }
    }
}
