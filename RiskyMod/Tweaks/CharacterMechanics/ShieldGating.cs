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
        public static bool disableIfGuardiansHeart = true;

        public ShieldGating()
        {
            IgnoreShieldGateDamage = DamageAPI.ReserveDamageType();
            if (!enabled) return;

            //Set this up after itemcatalog is loaded so that we can check if Guardian's heart exists
            RoR2Application.onLoad += SetupShieldGate;
        }

        private void SetupShieldGate()
        {
            if (disableIfGuardiansHeart && ItemCatalog.FindItemIndex("CLASSICITEMSRETURNS_ITEM_GUARDIANSHEART") != ItemIndex.None)
            {
                Debug.LogWarning("RiskyMod: Disabling ShieldGating since ClassicItemsReturns Guardian's Heart is loaded.");
                return;
            }

            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private void HealthComponent_TakeDamageProcess(ILContext il)
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
                    if (SneedUtils.SneedUtils.HasShieldOnly(self.body)) return remainingDamage;

                    bool bypassShield = (damageInfo.damageType & DamageType.BypassArmor) == DamageType.BypassArmor
                    || (damageInfo.damageType & DamageType.BypassOneShotProtection) == DamageType.BypassOneShotProtection
                    || (damageInfo.damageType & DamageType.BypassBlock) == DamageType.BypassBlock;

                    bool isPlayerTeam = self.body && self.body.teamComponent && (self.body.teamComponent.teamIndex == TeamIndex.Player || self.body.isPlayerControlled);

                    if (!bypassShield && isPlayerTeam)
                    {
                        bool attackerIsChampion = false;
                        if (damageInfo.attacker)
                        {
                            CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                            attackerIsChampion = attackerBody && attackerBody.isChampion;
                        }

                        if (!attackerIsChampion && !DamageAPI.HasModdedDamageType(damageInfo, IgnoreShieldGateDamage))
                        {
                            float duration = Time.fixedDeltaTime;
                            self.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, duration);
                            return 0f;
                        }
                    }
                    return remainingDamage;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("RiskyMod: ShieldGating TakeDamage IL Hook failed");
            }
        }
    }
}
