using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class ShieldGating
    {
        public static DamageAPI.ModdedDamageType IgnoreShieldGateDamage;
        public static bool enabled = true;

        public ShieldGating()
        {
            if (!enabled && !Items.Lunar.Transcendence.alwaysShieldGate) return;

            //Remove OSP in SharedHooks.RecalculateStats

            //Add Shield Gating
            IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdloc(8),
                     x => x.MatchLdarg(0),
                     x => x.MatchLdfld<HealthComponent>("shield"),
                     x => x.MatchSub(),
                     x => x.MatchStloc(8),
                     x => x.MatchLdarg(0)
                    ))
                {
                    c.Index += 4;
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_1);
                    c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((remainingDamage, self, damageInfo) =>
                    {
                        bool shieldOnly = self.body.HasBuff(RoR2Content.Buffs.AffixLunar)
                        || (self.body.inventory && self.body.inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0);

                        bool bypassShield = (damageInfo.damageType & DamageType.BypassArmor) == DamageType.BypassArmor
                        || (damageInfo.damageType & DamageType.BypassOneShotProtection) == DamageType.BypassOneShotProtection
                        || (damageInfo.damageType & DamageType.BypassBlock) == DamageType.BypassBlock;

                        bool cursed = self.body.cursePenalty > 1f;  //||self.body.isGlass

                        bool isPlayerTeam = self.body && self.body.teamComponent && (self.body.teamComponent.teamIndex == TeamIndex.Player || self.body.isPlayerControlled);

                        if (!bypassShield && isPlayerTeam)
                        {
                            bool isChampion = false;
                            if (damageInfo.attacker)
                            {
                                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                                isChampion = attackerBody && attackerBody.isChampion;
                            }

                            if (!(DamageAPI.HasModdedDamageType(damageInfo, IgnoreShieldGateDamage) || isChampion) || (shieldOnly && !cursed))
                            {
                                //This check lets transcendence shieldgate work when shieldgating is disabled.
                                if (ShieldGating.enabled || (shieldOnly && Items.Lunar.Transcendence.alwaysShieldGate))
                                {
                                    float duration = Time.fixedDeltaTime;

                                    //ShieldOnly increases grace period since it's your only form of defense against 1shots.
                                    if (shieldOnly)
                                    {
                                        duration = 0.5f;
                                    }

                                    self.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, duration);
                                    return 0f;
                                }
                            }
                        }
                        return remainingDamage;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: ShieldGating TakeDamage IL Hook failed");
                }
            };
        }
    }
}
