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
        private List<Assist> pendingAssists;
        private List<BanditAssist> pendingBanditAssists;
        //Refer to OnHitEnemy and OnCharacterDeath for assist application.

        public delegate void HandleAssist(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody);
        public static HandleAssist HandleAssistActions = HandleAssistMethod;
        private static void HandleAssistMethod(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody) { }

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

        public void AddBanditAssist(CharacterBody attackerBody, CharacterBody victimBody, float duration, BanditAssistType assistType)
        {
            //Check if this assist already exists.
            bool foundAssist = false;
            foreach (BanditAssist a in pendingBanditAssists)
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
                pendingBanditAssists.Add(new BanditAssist(attackerBody, victimBody, duration, assistType));
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
                            HandleAssistActions(a.attackerBody, attackerInventory, victimBody, killerBody);
                        }
                    }
                    pendingAssists.Remove(a);
                }
            }
        }

        private void TriggerBanditAssists(CharacterBody victimBody, CharacterBody killerBody, DamageInfo damageInfo)
        {
            if (pendingBanditAssists.Count > 0)
            {
                bool spawnedResetCooldownEffect = false;
                bool spawnedBanditSkullEffect = false;

                List<BanditAssist> toRemove = new List<BanditAssist>();
                foreach (BanditAssist a in pendingBanditAssists)
                {
                    if (a.victimBody == victimBody)
                    {
                        toRemove.Add(a);
                    }
                }

                foreach (BanditAssist a in toRemove)
                {
                    switch (a.assistType)
                    {
                        case BanditAssistType.BanditSkull:
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
                        case BanditAssistType.ResetCooldowns:
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
                        default:
                            break;
                    }
                }
            }
        }

        public void Awake()
        {
            pendingAssists = new List<Assist>();
            pendingBanditAssists = new List<BanditAssist>();
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

            if (pendingBanditAssists.Count > 0)
            {
                List<BanditAssist> toRemove = new List<BanditAssist>();
                foreach (BanditAssist a in pendingBanditAssists)
                {
                    a.timer -= Time.fixedDeltaTime;
                    if (a.timer <= 0 || !(a.attackerBody && a.attackerBody.healthComponent && a.attackerBody.healthComponent.alive))
                    {
                        toRemove.Add(a);
                    }
                }

                foreach (BanditAssist a in toRemove)
                {
                    pendingBanditAssists.Remove(a);
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

        private class BanditAssist
        {
            public float timer;
            public CharacterBody attackerBody;
            public CharacterBody victimBody;
            public BanditAssistType assistType;

            public BanditAssist(CharacterBody attackerBody, CharacterBody victimBody, float timer, BanditAssistType assistType)
            {
                this.attackerBody = attackerBody;
                this.victimBody = victimBody;
                this.assistType = assistType;
                this.timer = timer;
            }
        }

        public enum BanditAssistType
        {
            ResetCooldowns,
            BanditSkull
        }
    }
}
