using System.Collections.Generic;
using RiskyMod.Items.Common;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Uncommon;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Tweaks;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod
{
    public class AssistManager : MonoBehaviour
    {
        public static bool initialized = false;
        public static float assistLength = 3f;
        public static float directAssistLength = 1.2f;
        private List<Assist> pendingAssists;
        private List<DirectAssist> pendingDirectAssists;
        //Refer to OnHitEnemy and OnCharacterDeath for assist application.

        public delegate void HandleAssistInventory(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody);
        public static HandleAssistInventory HandleAssistInventoryActions = HandleAssistInventoryMethod;
        private static void HandleAssistInventoryMethod(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody) { }

        public void AddAssist(CharacterBody attackerBody, CharacterBody victimBody, float duration)
        {
            //Check if this assist already exists.
            bool foundAssist = false;
            foreach (Assist a in pendingAssists)
            {
                if (a.attackerBody == attackerBody && a.victimBody == victimBody)
                {
                    foundAssist = true;
                    if (a.timer < duration)
                    {
                        a.timer = duration;
                    }
                    break;
                }
            }

            if (!foundAssist)
            {
                pendingAssists.Add(new Assist(attackerBody, victimBody, duration));
            }
        }

        public void AddBanditAssist(CharacterBody attackerBody, CharacterBody victimBody, float duration, DirectAssistType assistType)
        {
            //Check if this assist already exists.
            bool foundAssist = false;
            foreach (DirectAssist a in pendingDirectAssists)
            {
                if (a.attackerBody == attackerBody && a.victimBody == victimBody && a.assistType == assistType)
                {
                    foundAssist = true;
                    if (a.timer < duration)
                    {
                        a.timer = duration;
                    }
                    break;
                }
            }

            if (!foundAssist)
            {
                pendingDirectAssists.Add(new DirectAssist(attackerBody, victimBody, duration, assistType));
            }
        }

        public void TriggerAssists(CharacterBody victimBody, CharacterBody killerBody, DamageInfo damageInfo)
        {
            //if (!NetworkServer.active) return; Thing that calls this is already gated behind a network check.
            if (BanditSpecialGracePeriod.enabled)
            {
                TriggerBanditAssists(victimBody, killerBody, damageInfo);
            }
            if (pendingAssists.Count > 0)
            {
                List<Assist> toRemove = new List<Assist>();
                foreach (Assist a in pendingAssists)
                {
                    if (a.victimBody == victimBody)
                    {
                        toRemove.Add(a);
                    }
                }

                foreach (Assist a in toRemove)
                {
                    if (a.attackerBody && a.attackerBody.healthComponent && a.attackerBody.healthComponent.alive)
                    {
                        Inventory attackerInventory = a.attackerBody.inventory;
                        if (attackerInventory)
                        {
                            HandleAssistInventoryActions(a.attackerBody, attackerInventory, victimBody, killerBody);
                        }
                    }
                    pendingAssists.Remove(a);
                }
            }
        }

        private void TriggerBanditAssists(CharacterBody victimBody, CharacterBody killerBody, DamageInfo damageInfo)
        {
            if (pendingDirectAssists.Count > 0)
            {
                bool spawnedResetCooldownEffect = false;
                bool spawnedBanditSkullEffect = false;

                List<DirectAssist> toRemove = new List<DirectAssist>();
                foreach (DirectAssist a in pendingDirectAssists)
                {
                    if (a.victimBody == victimBody)
                    {
                        toRemove.Add(a);
                    }
                }

                foreach (DirectAssist a in toRemove)
                {
                    switch (a.assistType)
                    {
                        case DirectAssistType.BanditSkull:
                            if (victimBody.master)
                            {
                                if (!spawnedBanditSkullEffect)
                                {
                                    spawnedBanditSkullEffect = true;
                                    EffectManager.SpawnEffect(BanditSpecialGracePeriod.skullEffect, new EffectData
                                    {
                                        origin = damageInfo.position
                                    }, true);
                                }
                                a.attackerBody.AddBuff(RoR2Content.Buffs.BanditSkull);
                            }
                            break;
                        case DirectAssistType.ResetCooldowns:
                            if (!spawnedResetCooldownEffect)
                            {
                                spawnedResetCooldownEffect = true;
                                EffectManager.SpawnEffect(BanditSpecialGracePeriod.resetEffect, new EffectData
                                {
                                    origin = damageInfo.position
                                }, true);
                            }
                            SkillLocator skillLocator = a.attackerBody.skillLocator;
                            if (skillLocator)
                            {
                                skillLocator.ResetSkills();
                            }
                            break;
                        case DirectAssistType.CrocoBiteHealOnKill:
                            if (a.attackerBody)
                            {
                                a.attackerBody.healthComponent.HealFraction(0.08f, default(ProcChainMask));
                                EffectData effectData = new EffectData
                                {
                                    origin = a.attackerBody.corePosition
                                };
                                effectData.SetNetworkedObjectReference(a.attackerBody.gameObject);
                                EffectManager.SpawnEffect(Survivors.SharedDamageTypes.medkitEffect, effectData, true);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void Awake()
        {
            pendingAssists = new List<Assist>();
            pendingDirectAssists = new List<DirectAssist>();
        }

        public void FixedUpdate()
        {
            UpdateAssistTimers();
        }

        private void UpdateAssistTimers()
        {
            if (pendingAssists.Count > 0)
            {
                List<Assist> toRemove = new List<Assist>();
                foreach (Assist a in pendingAssists)
                {
                    a.timer -= Time.fixedDeltaTime;
                    if (a.timer <= 0 || !(a.attackerBody && a.attackerBody.healthComponent && a.attackerBody.healthComponent.alive))
                    {
                        toRemove.Add(a);
                    }
                }

                foreach (Assist a in toRemove)
                {
                    pendingAssists.Remove(a);
                }
            }

            if (pendingDirectAssists.Count > 0)
            {
                List<DirectAssist> toRemove = new List<DirectAssist>();
                foreach (DirectAssist a in pendingDirectAssists)
                {
                    a.timer -= Time.fixedDeltaTime;
                    if (a.timer <= 0 || !(a.attackerBody && a.attackerBody.healthComponent && a.attackerBody.healthComponent.alive))
                    {
                        toRemove.Add(a);
                    }
                }

                foreach (DirectAssist a in toRemove)
                {
                    pendingDirectAssists.Remove(a);
                }
            }
        }

        private class Assist
        {
            public float timer;
            public CharacterBody attackerBody;
            public CharacterBody victimBody;

            public Assist(CharacterBody attackerBody, CharacterBody victimBody, float timer)
            {
                this.attackerBody = attackerBody;
                this.victimBody = victimBody;
                this.timer = timer;
            }
        }

        private class DirectAssist
        {
            public float timer;
            public CharacterBody attackerBody;
            public CharacterBody victimBody;
            public DirectAssistType assistType;

            public DirectAssist(CharacterBody attackerBody, CharacterBody victimBody, float timer, DirectAssistType assistType)
            {
                this.attackerBody = attackerBody;
                this.victimBody = victimBody;
                this.assistType = assistType;
                this.timer = timer;
            }
        }

        //Todo: convert from enum to a more general case
        public enum DirectAssistType
        {
            ResetCooldowns,
            BanditSkull,
            CrocoBiteHealOnKill
        }
    }
}