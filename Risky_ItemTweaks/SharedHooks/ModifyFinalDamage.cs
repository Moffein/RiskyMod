using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Risky_ItemTweaks.Items.Uncommon;
using R2API.Utils;
using Risky_ItemTweaks.Items.Legendary;

namespace Risky_ItemTweaks.SharedHooks
{
    public class ModifyFinalDamage
    {
        public static void Modify()
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
                                            newDamage *= 1f + 0.25f * lopperCount;
                                            damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;

                                            EffectManager.SpawnEffect(HealthComponent.AssetReferences.executeEffectPrefab, new EffectData
                                            {
                                                origin = victimBody.corePosition,
                                                scale = victimBody.radius * 0.5f
                                            }, true);
                                        }
                                    }
                                }
                                if (Headhunter.enabled)
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
