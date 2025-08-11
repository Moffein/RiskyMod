﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RiskyMod.Survivors.Croco.Contagion.Components;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod
{
    //Keep all the custom damagetypes in 1 place, in case there's a need to re-use them across different parts of the mod.
    public class SharedDamageTypes
    {
        public static DamageAPI.ModdedDamageType ResistedByAllies;
        public static DamageAPI.ModdedDamageType Ignite50Chance;
        public static DamageAPI.ModdedDamageType ProjectileRainForce;

        public static DamageAPI.ModdedDamageType AntiFlyingForce;
        public static DamageAPI.ModdedDamageType SawBarrier;

        public static DamageAPI.ModdedDamageType InterruptOnHit;

        public static DamageAPI.ModdedDamageType Slow50For5s;

        public static DamageAPI.ModdedDamageType CrocoBlightStack;
        public static DamageAPI.ModdedDamageType CrocoBlight6s;
        public static DamageAPI.ModdedDamageType CrocoPoison6s;

        public static DamageAPI.ModdedDamageType CaptainTaserSource;

        public static DamageAPI.ModdedDamageType AlwaysIgnite;   //Used for Molten Perforator due to not proccing

        public static DamageAPI.ModdedDamageType SweetSpotModifier;

        public static DamageAPI.ModdedDamageType DontTriggerBands;

        public static GameObject medkitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Medkit/MedkitHealEffect.prefab").WaitForCompletion();

        public SharedDamageTypes()
        {
            ResistedByAllies = DamageAPI.ReserveDamageType();
            SweetSpotModifier = DamageAPI.ReserveDamageType();   //Makes sweetspot falloff do -50% instead of -75%
            InterruptOnHit = DamageAPI.ReserveDamageType();
            ProjectileRainForce = DamageAPI.ReserveDamageType();

            AntiFlyingForce = DamageAPI.ReserveDamageType();

            CrocoBlightStack = DamageAPI.ReserveDamageType();
            CrocoBlight6s = DamageAPI.ReserveDamageType();
            CrocoPoison6s = DamageAPI.ReserveDamageType();

            AlwaysIgnite = DamageAPI.ReserveDamageType();

            Slow50For5s = DamageAPI.ReserveDamageType();
            
            CaptainTaserSource = DamageAPI.ReserveDamageType();
            DontTriggerBands = DamageAPI.ReserveDamageType();

            SawBarrier = DamageAPI.ReserveDamageType();

            TakeDamage.ModifyInitialDamageActions += ApplyProjectileRainForce;
            TakeDamage.ModifyInitialDamageActions += ApplyAntiFlyingForce;
            TakeDamage.ModifyInitialDamageActions += DisableBandProc;

            SneedHooks.ProcessHitEnemy.OnHitActions += ApplyInterruptOnHit;

            SneedHooks.ProcessHitEnemy.OnHitAttackerActions += ApplyCrocoBlight;
            SneedHooks.ProcessHitEnemy.OnHitAttackerActions += ApplyCrocoPoison;

            SneedHooks.ProcessHitEnemy.OnHitActions += ApplySlow50For5s;

            SneedHooks.ProcessHitEnemy.OnHitAttackerActions += ApplySawBarrierOnHit;
            SneedHooks.ProcessHitEnemy.OnHitAttackerActions += ApplyCaptainTaserSource;

            TakeDamage.OnDamageTakenAttackerActions += ApplyAlwaysIgnite;
            SetupSweetSpotModifier();
        }

        private void SetupSweetSpotModifier()
        {
            IL.RoR2.BlastAttack.HandleHits += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.75f)))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, BlastAttack, float>>((damagePenalty, self) =>
                    {
                        if (self.HasModdedDamageType(SweetSpotModifier))
                        {
                            damagePenalty *= 2f / 3f;
                        }
                        return damagePenalty;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: SweetSpotModifier DamateType IL Hook failed.");
                }
            };
        }

        private static void ApplySawBarrierOnHit(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SawBarrier))
            {
                if (attackerBody.healthComponent)
                {
                    attackerBody.healthComponent.AddBarrier(attackerBody.healthComponent.fullCombinedHealth * 0.003f);
                }
            }
        }

        private static void DisableBandProc(DamageInfo damageInfo, HealthComponent self)
        {
            if (damageInfo.HasModdedDamageType(DontTriggerBands))
            {
                damageInfo.procChainMask.AddProc(ProcType.Rings);
            }
        }

        private static void ApplyAlwaysIgnite(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(AlwaysIgnite))
            {
                float damageMultiplier = 0.5f;
                InflictDotInfo inflictDotInfo = new InflictDotInfo
                {
                    attackerObject = damageInfo.attacker,
                    victimObject = self.gameObject,
                    totalDamage = new float?(damageInfo.damage * damageMultiplier),
                    damageMultiplier = 1f,
                    dotIndex = DotController.DotIndex.Burn,
                    maxStacksFromAttacker = null
                };
                if (attackerBody.inventory)
                {
                    StrengthenBurnUtils.CheckDotForUpgrade(attackerBody.inventory, ref inflictDotInfo);
                }
                DotController.InflictDot(ref inflictDotInfo);
            }
        }

        private static void ApplyProjectileRainForce(DamageInfo damageInfo, HealthComponent self)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.ProjectileRainForce))
            {
                if (damageInfo.inflictor && damageInfo.inflictor.transform)
                {
                    Vector3 direction = -damageInfo.inflictor.transform.up;
                    CharacterBody cb = self.body;
                    if (cb)
                    {
                        //Scale force to match mass
                        Rigidbody rb = cb.rigidbody;
                        if (rb)
                        {
                            direction *= Mathf.Max(rb.mass / 100f, 1f);
                        }
                    }
                    damageInfo.force = 330f * direction;
                }
            }
        }

        private static void ApplyInterruptOnHit(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.InterruptOnHit))
            {
                SetStateOnHurt component = victimBody.healthComponent.GetComponent<SetStateOnHurt>();
                if (component != null)
                {
                    component.SetStun(-1f);
                }
            }
        }

        private static void ApplyCrocoBlight(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.CrocoBlight6s))
            {
                float duration = 6f;
                int stacks = 1;
                if (damageInfo.HasModdedDamageType(SharedDamageTypes.CrocoBlightStack)) stacks = Mathf.CeilToInt(damageInfo.damage);

                for (int i = 0; i < stacks; i++)
                {
                    DotController.InflictDot(victimBody.gameObject, damageInfo.attacker, DotController.DotIndex.Blight, duration, 1f);

                    if (i > 0 && GlobalContagionTracker.instance)
                    {
                        GlobalContagionTracker.instance.Add(attackerBody, victimBody, SharedDamageTypes.CrocoBlight6s, duration);
                    }
                }
            }
        }
        private static void ApplyCrocoPoison(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.CrocoPoison6s))
            {
                DotController.InflictDot(victimBody.gameObject, damageInfo.attacker, DotController.DotIndex.Poison, 6f, 1f);
            }
        }

        private static void ApplyAntiFlyingForce(DamageInfo damageInfo, HealthComponent self)
        {
            if (damageInfo.HasModdedDamageType(AntiFlyingForce))
            {
                Vector3 direction = Vector3.down;
                CharacterBody cb = self.body;
                if (cb && cb.isFlying)
                {
                    //Scale force to match mass
                    Rigidbody rb = cb.rigidbody;
                    if (rb)
                    {
                        if (damageInfo.force.y > 0f)
                        {
                            damageInfo.force.y = 0f;
                        }

                        direction *= Mathf.Min(4f, Mathf.Max(rb.mass / 100f, 1f));  //Greater Wisp 300f, SCU 1000f
                        damageInfo.force += 1600f * direction;
                    }
                }
            }
        }

        private static void ApplySlow50For5s(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(Slow50For5s))
            {
                victimBody.AddTimedBuff(RoR2Content.Buffs.Slow50, 5f);
            }
        }

        private static void ApplyCaptainTaserSource(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SharedDamageTypes.CaptainTaserSource))
            {
                List<HealthComponent> bouncedObjects = new List<HealthComponent>();
                bouncedObjects.Add(victimBody.healthComponent);

                int initialTargets = 2; //Since the range is smaller than Epidemic, add more initial targets.
                float range = 15f;

                //Need to individually find all targets for the first bounce.
                for (int i = 0; i < initialTargets; i++)
                {
                    LightningOrb taserLightning = new LightningOrb
                    {
                        bouncedObjects = bouncedObjects,
                        attacker = damageInfo.attacker,
                        inflictor = damageInfo.attacker,
                        damageValue = damageInfo.damage,
                        procCoefficient = 1f,
                        teamIndex = attackerBody.teamComponent.teamIndex,
                        isCrit = damageInfo.crit,
                        procChainMask = damageInfo.procChainMask,
                        lightningType = LightningOrb.LightningType.Ukulele,
                        damageColorIndex = DamageColorIndex.Default,
                        bouncesRemaining = 2,
                        targetsToFindPerBounce = 1,
                        range = range,
                        origin = damageInfo.position,
                        damageType = DamageType.Shock5s,
                        speed = 120f
                    };
                    //taserLightning.AddModdedDamageType(SharedDamageTypes.Slow50For5s);

                    //2 initial lightnings
                    //Each lightning will hit, then bounce 2 extra times

                    HurtBox hurtBox = taserLightning.PickNextTarget(victimBody.corePosition);

                    //Fire orb if HurtBox is found.
                    if (hurtBox)
                    {
                        taserLightning.target = hurtBox;
                        OrbManager.instance.AddOrb(taserLightning);
                        taserLightning.bouncedObjects.Add(hurtBox.healthComponent);
                    }
                }
            }
        }
    }
}
