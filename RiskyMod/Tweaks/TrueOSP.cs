using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks
{
    public class TrueOSP
    {
        public static bool enabled = true;
        public static BuffDef DisableOSP;
        public static bool enableLogging = false;

        public TrueOSP()
        {
            if (!enabled) return;

            DisableOSP = ScriptableObject.CreateInstance<BuffDef>();
            DisableOSP.buffColor = new Color(0.9f * 140f / 255f, 0.9f * 185f / 255f, 0.9f * 191f / 255f);
            DisableOSP.canStack = false;
            DisableOSP.isDebuff = false;    //Not a debuff so that it doesn't interact with debuff-related stuff like Death Mark and Blast Shower.
            DisableOSP.name = "RiskyMod_DisableOSPBuff";
            DisableOSP.iconSprite = LegacyResourcesAPI.Load<Sprite>("textures/bufficons/texBuffPulverizeIcon");
            R2API.ContentAddition.AddBuffDef((DisableOSP));

            //Handled in MonoBehaviours.OSPManagerComponent and RecalculateStats
            On.RoR2.CharacterBody.RecalculateStats += HandleDisableOSPBuff;

            //Overwrite vanilla OSP handling
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                //Remove vanilla OSP check
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<CharacterBody>("get_hasOneShotProtection")
                    );
                c.Index++;
                c.EmitDelegate<Func<bool, bool>>((hasOSP) =>
                {
                    return false;
                });

                //Check for OSP at the end of applying health damage
                c.GotoNext(
                     x => x.MatchCall<HealthComponent>("set_Networkhealth")
                    );
                c.Emit(OpCodes.Ldarg_0);//healthcomponent
                c.Emit(OpCodes.Ldarg_1);//damageinfo
                c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((finalHealth, self, damageInfo) =>
                {
                    if (self.body.hasOneShotProtection && (damageInfo.damageType & DamageType.BypassOneShotProtection) != DamageType.BypassOneShotProtection)
                    {
                        OSPComponent ospm = self.body.gameObject.GetComponent<OSPComponent>();
                        if (!ospm)
                        {
                            ospm = self.body.gameObject.AddComponent<OSPComponent>();
                            ospm.healthComponent = self;
                            ospm.characterBody = self.body;
                            if (enableLogging) Debug.Log("Adding OSP Manager to player.");
                        }
                        //Check if OSP timer should be triggered
                        float ospHealthThreshold = self.fullHealth * OSPComponent.ospThreshold;
                        if (self.health >= ospHealthThreshold && finalHealth < ospHealthThreshold)
                        {
                            if (!ShieldGating.enabled || self.shield <= 0f)
                            {
                                ospm.StartOSPTimer();
                            }
                        }
                        if (finalHealth <= 0f)
                        {
                            if (ospm.TriggerOSP())
                            {
                                finalHealth = 1f;
                            }
                        }
                    }
                    return finalHealth;
                });
            };
        }

        private static void HandleDisableOSPBuff(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.hasOneShotProtection)
            {
                //Disable vanilla OSP
                self.oneShotProtectionFraction = 0f;    //I'd like to re-enable the visual, but I need to figure out how to make it not count shields.

                if (self.HasBuff(TrueOSP.DisableOSP))
                {
                    if (NetworkServer.active && self.outOfDanger)// && (self.healthComponent && self.healthComponent.health/self.healthComponent.fullHealth > OSPManagerComponent.ospThreshold)
                    {
                        self.RemoveBuff(TrueOSP.DisableOSP);
                    }
                    else
                    {
                        self.hasOneShotProtection = false;
                    }
                }
            }
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
                if (ospStopwatch <= 0f)
                {
                    if (TrueOSP.enableLogging) Debug.Log("OSP timer expired.");
                }
            }
        }

        public void StartOSPTimer()
        {
            if (ospStopwatch <= 0f && CanTriggerOSP())
            {
                ospStopwatch = ospTimer;
                if (TrueOSP.enableLogging) Debug.Log("Starting OSP timer.");
            }
        }

        public bool TriggerOSP()
        {
            bool ospTriggered = false;
            if (CanTriggerOSP())
            {
                ospTriggered = true;
                ospStopwatch = 0f;
                characterBody.AddBuff(TrueOSP.DisableOSP);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, ospInvuln);
                characterBody.outOfDangerStopwatch = 0f;
                if (TrueOSP.enableLogging) Debug.Log("Triggering OSP");
            }
            return ospTriggered;
        }

        public bool CanTriggerOSP()
        {
            return !characterBody.HasBuff(TrueOSP.DisableOSP) && (healthComponent.health / healthComponent.fullHealth > ospThreshold || ospStopwatch > 0f);
        }
    }
}
