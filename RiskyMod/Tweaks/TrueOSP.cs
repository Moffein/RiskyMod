using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.MonoBehaviours;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    class TrueOSP
    {
        public static bool enabled = true;
        public static BuffDef disableOSP;
        public TrueOSP()
        {
            if (!enabled) return;

            disableOSP = ScriptableObject.CreateInstance<BuffDef>();
            disableOSP.buffColor = new Color(0.8f * 140f / 255f, 0.8f * 185f / 255f, 0.8f * 191f / 255f);
            disableOSP.canStack = false;
            disableOSP.isDebuff = false;
            disableOSP.name = "RiskyItemTweaks_DisableOSPBuff";
            disableOSP.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffPulverizeIcon");
            BuffAPI.Add(new CustomBuff(disableOSP));

            //Handled in MonoBehaviours.OSPManagerComponent and SharedHooks.RecalculateStats

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
                        OSPManagerComponent ospm = self.body.gameObject.GetComponent<OSPManagerComponent>();
                        if (!ospm)
                        {
                            ospm = self.body.gameObject.AddComponent<OSPManagerComponent>();
                            ospm.healthComponent = self;
                            ospm.characterBody = self.body;
                            Debug.Log("Adding OSP Manager to player.");
                        }
                        //Check if OSP timer should be triggered
                        float ospHealthThreshold = self.fullHealth * OSPManagerComponent.ospThreshold;
                        if (self.health >= ospHealthThreshold && finalHealth < ospHealthThreshold)
                        {
                            ospm.StartOSPTimer();
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
    }
}
