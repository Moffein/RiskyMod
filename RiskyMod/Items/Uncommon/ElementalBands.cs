using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.Items.Common;
using RoR2;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class ElementalBands
    {
        public static bool enabled = true;

        public static float initialDamageCoefficientFire = 2.5f;
        public static float stackDamageCoefficientFire = 1.5f;

        public static float initialDamageCoefficientIce = 2f;
        public static float stackDamageCoefficientIce = 1.2f;

        public ElementalBands()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            float initialDamageFire = initialDamageCoefficientFire - stackDamageCoefficientFire;
            float initialDamageIce = initialDamageCoefficientIce - stackDamageCoefficientIce;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);

                //Change ring threshold
                c.GotoNext(MoveType.After,
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "ElementalRingsReady"),
                     x => x.MatchCallvirt<CharacterBody>("HasBuff")
                    );
                c.Emit(OpCodes.Ldarg_1);//damageinfo
                c.Emit(OpCodes.Ldloc_1);//attacker body
                c.EmitDelegate<Func<bool, DamageInfo, CharacterBody, bool>>((activateRings, damageInfo, attackerBody) =>
                {
                    if (activateRings)
                    {
                        if (Crowbar.enabled && DamageAPI.HasModdedDamageType(damageInfo, Crowbar.CrowbarDamage) && damageInfo.attacker && attackerBody)
                        {
                            float ringThreshold = 4f;
                            Inventory inv = attackerBody.inventory;
                            if (inv)
                            {
                                int crowbarCount = inv.GetItemCount(RoR2Content.Items.Crowbar);
                                if (crowbarCount > 0)
                                {
                                    ringThreshold *= Crowbar.GetCrowbarMult(crowbarCount);
                                }
                            }

                            if (damageInfo.damage / attackerBody.damage < ringThreshold)
                            {
                                activateRings = false;
                            }
                        }

                        if (damageInfo.damageType.HasFlag(DamageType.DoT)) activateRings = false;
                    }
                    return activateRings;
                });

                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "IceRing")
                    );

                //Change IceRing damage
                c.GotoNext(
                     x => x.MatchLdcR4(2.5f)
                    );
                c.Next.Operand = stackDamageCoefficientIce;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamageIce;
                });

                //Jump to FireRing
                c.GotoNext(
                     x => x.MatchLdstr("Prefabs/Projectiles/FireTornado")
                    );

                //Change FireRing damage
                c.GotoNext(
                     x => x.MatchLdcR4(3f)
                    );
                c.Next.Operand = stackDamageCoefficientFire;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamageFire;
                });
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.IceRing);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.FireRing);
        }
    }
}
