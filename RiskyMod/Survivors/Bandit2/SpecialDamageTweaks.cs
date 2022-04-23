using RiskyMod.SharedHooks;
using RoR2;
using R2API;
using EntityStates.RiskyMod.Bandit2.Revolver;
using MonoMod.Cil;
using System;
using Mono.Cecil.Cil;
using UnityEngine;
using RoR2.Orbs;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace RiskyMod.Survivors.Bandit2
{
    public class SpecialDamageTweaks
    {
        public static GameObject ricochetOrbEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/ChainGunOrbEffect.prefab").WaitForCompletion();
        public SpecialDamageTweaks()
        {
            OnHitEnemy.OnHitAttackerActions += RicochetBullet;
            OnHitEnemy.OnHitNoAttackerActions += ApplyBuff;
            TakeDamage.ModifyInitialDamageActions += RackEmUpBonus;
        }

        private static void RicochetBullet(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.RevolverRicochet))
            {
                ChainGunOrb chainGunOrb = new ChainGunOrb(ricochetOrbEffect);
                chainGunOrb.damageValue = damageInfo.damage;
                chainGunOrb.isCrit = damageInfo.crit;
                chainGunOrb.teamIndex = attackerBody.teamComponent ? attackerBody.teamComponent.teamIndex : TeamIndex.None;
                chainGunOrb.attacker = damageInfo.attacker;
                chainGunOrb.procCoefficient = damageInfo.procCoefficient;
                chainGunOrb.procChainMask = damageInfo.procChainMask;
                chainGunOrb.origin = damageInfo.position;
                chainGunOrb.speed = 200f;   //Drone Parts is 600f
                chainGunOrb.bouncesRemaining = 1;
                chainGunOrb.bounceRange = 20f;
                chainGunOrb.damageCoefficientPerBounce = 0.75f;
                chainGunOrb.targetsToFindPerBounce = 1;
                chainGunOrb.canBounceOnSameTarget = false;
                chainGunOrb.damageColorIndex = damageInfo.damageColorIndex;

                chainGunOrb.damageType = damageInfo.damageType;

                if (damageInfo.HasModdedDamageType(Bandit2Core.ResetRevolverOnKill)) chainGunOrb.AddModdedDamageType(Bandit2Core.ResetRevolverOnKill);
                if (damageInfo.HasModdedDamageType(Bandit2Core.RackEmUpDamage)) chainGunOrb.AddModdedDamageType(Bandit2Core.RackEmUpDamage);
                if (damageInfo.HasModdedDamageType(Bandit2Core.SpecialDamage)) chainGunOrb.AddModdedDamageType(Bandit2Core.SpecialDamage);

                chainGunOrb.bouncedObjects = new List<HealthComponent>();
                if (victimBody && victimBody.healthComponent)
                {
                    chainGunOrb.bouncedObjects.Add(victimBody.healthComponent);
                }
                chainGunOrb.target = chainGunOrb.PickNextTarget(chainGunOrb.origin);

                if (chainGunOrb.target) OrbManager.instance.AddOrb(chainGunOrb);
            }
        }

        private static void RackEmUpBonus(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.RackEmUpDamage))
            {
                float mult = 1f + self.body.GetBuffCount(Bandit2Core.SpecialDebuff) * (FireRackEmUp.bonusDamageCoefficient / FireRackEmUp.damageCoefficient);
                damageInfo.damage *= mult;
            }
        }

        private static void ApplyBuff(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.SpecialDamage))
            {
                float buffDuration = BanditSpecialGracePeriod.enabled ? BanditSpecialGracePeriod.duration : 1.2f;
                int specialCount = victimBody.GetBuffCount(Bandit2Core.SpecialDebuff) + 1;
                victimBody.ClearTimedBuffs(Bandit2Core.SpecialDebuff);
                for (int i = 0; i < specialCount; i++)
                {
                    victimBody.AddTimedBuff(Bandit2Core.SpecialDebuff, buffDuration);
                }
            }
        }
    }
}
