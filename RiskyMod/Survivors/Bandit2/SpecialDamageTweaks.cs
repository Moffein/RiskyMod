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
using static AssistManager.AssistManager;

namespace RiskyMod.Survivors.Bandit2
{
    public class SpecialDamageTweaks
    {
        private static GameObject skullEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2KillEffect.prefab").WaitForCompletion();
        public SpecialDamageTweaks()
        {
            OnHitEnemy.OnHitNoAttackerActions += ApplyBuff;
            TakeDamage.ModifyInitialDamageNoAttackerActions += RackEmUpBonus;

            //Standoff
            On.RoR2.GlobalEventManager.OnCharacterDeath += ProcStandoffOnKill;
            OnHitEnemy.PreOnHitAttackerActions += AddStandoffAssist;
            AssistManager.AssistManager.HandleDirectAssistActions += HandleStandoffAssist;
        }

        private void HandleStandoffAssist(Assist assist, CharacterBody killerBody, DamageInfo damageInfo)
        {
            if (assist.damageType == null
                && assist.moddedDamageTypes.Contains(Bandit2Core.StandoffDamage)
                && assist.moddedDamageTypes.Count == 1)
            {
                bool isStandoff = damageInfo.HasModdedDamageType(Bandit2Core.StandoffDamage);

                if (!(isStandoff && killerBody == assist.attackerBody))
                {
                    Bandit2Core.ApplyStandoff(assist.attackerBody);
                    if (!isStandoff && !damageInfo.damageType.damageType.HasFlag(DamageType.GiveSkullOnKill))
                    {
                        EffectManager.SpawnEffect(skullEffect, new EffectData
                        {
                            origin = damageInfo.position
                        }, true);
                    }
                }
            }
        }

        private void AddStandoffAssist(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (!damageInfo.HasModdedDamageType(Bandit2Core.StandoffDamage)) return;
            Assist standoffAssist = new Assist(attackerBody, victimBody, AssistManager.AssistManager.GetDirectAssistDurationForAttacker(attackerBody.gameObject));
            standoffAssist.moddedDamageTypes.Add(Bandit2Core.StandoffDamage);
            AssistManager.AssistManager.instance.AddDirectAssist(standoffAssist);
        }

        private void ProcStandoffOnKill(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            if (damageReport.damageInfo != null && damageReport.damageInfo.HasModdedDamageType(Bandit2Core.StandoffDamage) && damageReport.attackerBody)
            {
                Bandit2Core.ApplyStandoff(damageReport.attackerBody);
                if (!damageReport.damageInfo.damageType.damageType.HasFlag(DamageType.GiveSkullOnKill))
                {
                    EffectManager.SpawnEffect(skullEffect, new EffectData
                    {
                        origin = damageReport.damageInfo.position
                    }, true);
                }
            }
        }

        private static void RackEmUpBonus(DamageInfo damageInfo, HealthComponent self)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.RackEmUpDamage))
            {
                float mult = 1f + self.body.GetBuffCount(Bandit2Core.SpecialDebuff) * (FireRackEmUp.bonusDamageCoefficient / FireRackEmUp.damageCoefficient);
                damageInfo.damage *= mult;
            }
        }

        //Refreshes buff count while repeatedly hitting enemies for Rack Em Up.
        private static void ApplyBuff(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.RackEmUpDamage)) //was SpecialDamage
            {
                float buffDuration = 0.5f;  //used to show up as the special grace period buff
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
