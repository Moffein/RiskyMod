using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RiskyMod.Items.Uncommon;
using R2API.Utils;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Common;

namespace RiskyMod.SharedHooks
{
    public class ModifyFinalDamage
    {
        public ModifyFinalDamage()
        {
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
					 x => x.MatchLdarg(1),
					 x => x.MatchLdfld<DamageInfo>("damage"),
                     x => x.MatchStloc(6)
					);
				c.Index ++;
                c.Remove();
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<DamageInfo, HealthComponent, float>>((damageInfo, victimHealth) =>
                {
                    float newDamage = damageInfo.damage;
                    CharacterBody victimBody = victimHealth.body;
                    if (victimBody && damageInfo.attacker)
                    {
                        CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                        if (attackerBody)
                        {
                            Inventory attackerInventory = attackerBody.inventory;
                            if (attackerInventory)
                            {
                                if (Guillotine.enabled)
                                {
                                    int lopperCount = attackerInventory.GetItemCount(RoR2Content.Items.ExecuteLowHealthElite);
                                    if (lopperCount > 0)
                                    {
                                        if (victimHealth.combinedHealth <= victimHealth.fullCombinedHealth * 0.3f)
                                        {
                                            newDamage *= 1f + Guillotine.damageCoefficient * lopperCount;
                                            damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;

                                            //Lock the visual effect behind proccing attacks to improve performance
                                            if (damageInfo.procCoefficient > 0f)
                                            {
                                                EffectManager.SpawnEffect(HealthComponent.AssetReferences.executeEffectPrefab, new EffectData
                                                {
                                                    origin = victimBody.corePosition,
                                                    scale = victimBody.radius * 0.3f * damageInfo.procCoefficient   //not sure if radius is even getting affected
                                                }, true);
                                            }
                                        }
                                    }
                                }
                                if (HeadHunter.enabled)
                                {
                                    int hhCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
                                    if (hhCount > 0)
                                    {
                                        if (victimBody.isElite)
                                        {
                                            newDamage *= 1.3f;
                                            damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return newDamage;
                });
            };
		}
    }
}
