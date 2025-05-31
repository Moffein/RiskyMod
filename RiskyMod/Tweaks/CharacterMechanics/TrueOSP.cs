using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class TrueOSP
    {
        public static bool enabled = true;
        public static BuffDef DisableOSP;

        public TrueOSP()
        {
            if (!enabled) return;

            DisableOSP = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_DisableOSPBuff",
                false,
                false,
                false,
                new Color(0.9f * 140f / 255f, 0.9f * 185f / 255f, 0.9f * 191f / 255f),
                 Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ArmorReductionOnHit/bdPulverized.asset").WaitForCompletion().iconSprite
                );

            SharedHooks.RecalculateStats.HandleRecalculateStatsActions += HandleTrueOSP;
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private static void HandleTrueOSP(CharacterBody self)
        {

            if (self.hasOneShotProtection)
            {
                //Can't get ShieldOnly to work so just don't handle it.
                if (!SneedUtils.SneedUtils.HasShieldOnly(self))
                {
                    //Disable vanilla OSP
                    //I'd like to re-enable the visual, but I need to figure out how to make it not count shields.
                    self.oneShotProtectionFraction = 0f;

                    if (self.cursePenalty > 1f)
                    {
                        self.hasOneShotProtection = false;
                    }
                }

                if (self.HasBuff(DisableOSP))
                {
                    if (NetworkServer.active && self.outOfDanger)
                    {
                        self.RemoveBuff(DisableOSP);
                    }
                    else
                    {
                        self.hasOneShotProtection = false;
                    }
                }
            }
        }

        private void HealthComponent_TakeDamageProcess(MonoMod.Cil.ILContext il)
        {
            bool error = true;

            //Remove vanilla OSP check
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(
                 x => x.MatchCallvirt<CharacterBody>("get_hasOneShotProtection")
                ))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);//healthcomponent
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((hasOSP, self) =>
                {
                    return SneedUtils.SneedUtils.HasShieldOnly(self.body) && hasOSP;
                });

                //Check for OSP at the end of applying health damage
                if (c.TryGotoNext(
                 x => x.MatchCall<HealthComponent>("set_Networkhealth")
                ))
                {
                    c.Emit(OpCodes.Ldarg_0);//healthcomponent
                    c.Emit(OpCodes.Ldarg_1);//damageinfo
                    c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>(ProcTrueOSP_Health);
                    error = false;
                }
            }
            if (error)
            {
                UnityEngine.Debug.LogError("TrueOSP IL Hook failed");
            }
        }

        private float ProcTrueOSP_Health(float finalHealth, HealthComponent self, DamageInfo damageInfo)
        {
            if (!(self.body.hasOneShotProtection && (damageInfo.damageType & DamageType.BypassOneShotProtection) != DamageType.BypassOneShotProtection)) return finalHealth;

            //can't get shieldonly to work so just default to vanilla behavior
            if (SneedUtils.SneedUtils.HasShieldOnly(self.body)) return finalHealth;

            OSPComponent ospm = self.body.gameObject.GetComponent<OSPComponent>();
            if (!ospm)
            {
                ospm = self.body.gameObject.AddComponent<OSPComponent>();
                ospm.healthComponent = self;
                ospm.characterBody = self.body;
            }

            //Check if OSP timer should be triggered

            float ospHealthThreshold = self.fullHealth * OSPComponent.ospThreshold;
            if (self.health >= ospHealthThreshold && finalHealth < ospHealthThreshold)
            {
                ospm.StartOSPTimer();
            }

            //Save player from death
            if (finalHealth <= 0f && ospm.TriggerOSP())
            {
                finalHealth = 1f;
            }
            return finalHealth;
        }
    }

    public class OSPComponent : MonoBehaviour
    {
        public static float ospThreshold = 0.9f;    //Max HP Percent that can trigger OSP
        public static float ospTimer = 0.5f;    //After HP goes below ospThreshold, taking lethal damage within this timeframe will trigger OSP.
        public static float ospInvuln = 0.5f;   //Grace period after triggering OSP

        //These will be set during TakeDamage
        public CharacterBody characterBody = null;
        public HealthComponent healthComponent = null;

        private float ospStopwatch;

        public void Awake()
        {
            ospStopwatch = 0f;
        }

        public void FixedUpdate()
        {
            if (ospStopwatch > 0f)
            {
                ospStopwatch -= Time.fixedDeltaTime;
            }
        }

        public void StartOSPTimer()
        {
            if (CanTriggerOSP())
            {
                ospStopwatch = ospTimer;
            }
        }

        public bool TriggerOSP()
        {
            bool ospTriggered = false;
            if (CanTriggerOSP() && ospStopwatch > 0f)
            {
                ospTriggered = true;
                ospStopwatch = 0f;
                characterBody.AddBuff(TrueOSP.DisableOSP);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, ospInvuln);
                characterBody.outOfDangerStopwatch = 0f;
            }
            return ospTriggered;
        }

        public bool CanTriggerOSP()
        {
            return !characterBody.HasBuff(TrueOSP.DisableOSP);
        }
    }
}