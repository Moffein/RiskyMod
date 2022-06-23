using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RiskyMod.Items.Uncommon;
using R2API.Utils;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Common;
using UnityEngine;

namespace RiskyMod.SharedHooks
{
    public class ModifyFinalDamage
    {
        public delegate void ModifyFinalDamageDelegate(DamageMult damageMult, DamageInfo damageInfo,
            HealthComponent victim, CharacterBody victimBody,
            CharacterBody attackerBody, Inventory attackerInventory);
        public static ModifyFinalDamageDelegate ModifyFinalDamageActions;

        public ModifyFinalDamage()
        {
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);
				if(c.TryGotoNext(
                     x => x.MatchLdarg(1),
                     x => x.MatchLdfld<DamageInfo>("damage"),
                     x => x.MatchStloc(6)
                    ))
                {
                    c.Index += 3;
                    c.Emit(OpCodes.Ldloc, 6);
                    c.Emit(OpCodes.Ldarg_0);    //self
                    c.Emit(OpCodes.Ldarg_1);    //damageInfo
                    c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((origDamage, victimHealth, damageInfo) =>
                    {
                        float newDamage = origDamage;
                        CharacterBody victimBody = victimHealth.body;
                        if (victimBody && damageInfo.attacker)
                        {
                            CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                            if (attackerBody)
                            {
                                Inventory attackerInventory = attackerBody.inventory;
                                if (attackerInventory)
                                {
                                    DamageMult damageMult = new DamageMult();
                                    if (ModifyFinalDamageActions != null)
                                    {
                                        ModifyFinalDamageActions.Invoke(damageMult, damageInfo, victimHealth, victimBody, attackerBody, attackerInventory);
                                        newDamage *= damageMult.damageMult;
                                    }
                                }
                            }
                        }
                        return newDamage;
                    });
                    c.Emit(OpCodes.Stloc, 6);
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: ModifyFinalDamage IL Hook failed. This will break a lot of things.");
                }
            };
		}
    }

    public class DamageMult
    {
        public float damageMult = 1f;
    }
}
