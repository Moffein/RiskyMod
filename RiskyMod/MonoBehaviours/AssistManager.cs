using System.Collections.Generic;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Uncommon;
using RiskyMod.Tweaks;
using RoR2;
using UnityEngine;

namespace RiskyMod.MonoBehaviours
{
    public class AssistManager : MonoBehaviour
    {
        public static bool initialized = false;
        public static float genericAssistLength = 3f;
        private List<Assist> pendingAssists;

        public void AddAssist(CharacterBody attackerBody, CharacterBody victimBody, AssistType assistType, float duration)
        {
            //Check if this assist already exists.
            bool foundAssist = false;
            foreach (Assist a in pendingAssists)
            {
                if (a.attackerBody == attackerBody && a.victimBody == victimBody && a.assistType == assistType)
                {
                    foundAssist = true;
                    if (a.timer < duration)
                    {
                        a.timer = duration;
                    }
                }
            }

            if (!foundAssist)
            {
                pendingAssists.Add(new Assist(attackerBody, victimBody, assistType, duration));
            }
        }

        public void TriggerAssists(CharacterBody victimBody, CharacterBody killerBody, Vector3 position)
        {
            if (pendingAssists.Count > 0)
            {
                bool spawnedResetCooldownEffect = false;
                bool spawnedBanditSkullEffect = false;

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
                        switch (a.assistType)
                        {
                            case AssistType.Berzerk:
                                if (Berzerker.enabled)
                                {
                                    Inventory attackerInventory = a.attackerBody.inventory;
                                    if (attackerInventory)
                                    {
                                        int berzerkCount = attackerInventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
                                        if (berzerkCount > 0)
                                        {
                                            //Need to apply buff this way to prevent the visual from disappearing.
                                            int newBuffStack = Mathf.Min(a.attackerBody.GetBuffCount(Berzerker.berzerkBuff) + 1, 2 + 3 * berzerkCount);
                                            int foundBuffs = 0;
                                            foreach (CharacterBody.TimedBuff tb in a.attackerBody.timedBuffs)
                                            {
                                                if (tb.buffIndex == Berzerker.berzerkBuff.buffIndex)
                                                {
                                                    tb.timer = 6f + foundBuffs;
                                                    foundBuffs++;
                                                }
                                            }
                                            for (int i = 0; i < newBuffStack - foundBuffs; i++)
                                            {
                                                a.attackerBody.AddTimedBuff(Berzerker.berzerkBuff, 6f + foundBuffs);
                                                foundBuffs++;
                                            }
                                        }
                                    }
                                }
                                break;
                            case AssistType.HeadHunter:
                                if (HeadHunter.enabled)
                                {
                                    Inventory attackerInventory = a.attackerBody.inventory;
                                    if (attackerInventory)
                                    {
                                        int hhCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
                                        if (hhCount > 0)
                                        {
                                            float duration = 5f + 5f * hhCount;
                                            for (int l = 0; l < BuffCatalog.eliteBuffIndices.Length; l++)
                                            {
                                                BuffIndex buffIndex = BuffCatalog.eliteBuffIndices[l];
                                                if (victimBody.HasBuff(buffIndex))
                                                {
                                                    a.attackerBody.AddTimedBuff(buffIndex, duration);
                                                    a.attackerBody.AddTimedBuff(HeadHunter.headhunterBuff.buffIndex, duration);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case AssistType.Brainstalks:
                                if (Brainstalks.enabled)
                                {
                                    Inventory attackerInventory = a.attackerBody.inventory;
                                    if (attackerInventory)
                                    {
                                        int bsCount = attackerInventory.GetItemCount(RoR2Content.Items.KillEliteFrenzy);
                                        if (bsCount > 0)
                                        {
                                            a.attackerBody.AddTimedBuff(RoR2Content.Buffs.NoCooldowns, bsCount * 4f);
                                        }
                                    }
                                }
                                break;
                            case AssistType.LaserTurbine:
                                //Vanilla behavior is left functional since the actual behavior is only 1 line of code.
                                if (a.attackerBody != killerBody)
                                {
                                    a.attackerBody.AddTimedBuff(RoR2Content.Buffs.LaserTurbineKillCharge, EntityStates.LaserTurbine.RechargeState.killChargeDuration, EntityStates.LaserTurbine.RechargeState.killChargesRequired);
                                }
                                break;
                            case AssistType.BanditSkull:
                                if (BanditSpecialGracePeriod.enabled && victimBody.master)
                                {
                                    if (!spawnedBanditSkullEffect)
                                    {
                                        EffectManager.SpawnEffect(BanditSpecialGracePeriod.skullEffect, new EffectData
                                        {
                                            origin = position
                                        }, true);
                                    }
                                    a.attackerBody.AddBuff(RoR2Content.Buffs.BanditSkull);
                                }
                                break;
                            case AssistType.ResetCooldowns:
                                if (BanditSpecialGracePeriod.enabled)
                                {
                                    if (!spawnedResetCooldownEffect)
                                    {
                                        EffectManager.SpawnEffect(BanditSpecialGracePeriod.resetEffect, new EffectData
                                        {
                                            origin = position
                                        }, true);
                                    }
                                    SkillLocator skillLocator = a.attackerBody.skillLocator;
                                    if (skillLocator)
                                    {
                                        skillLocator.ResetSkills();
                                    }
                                }
                                break;
                            case AssistType.FrostRelic:
                                //Vanilla behavior is left functional since it requires a GetComponent to access.
                                if (a.attackerBody != killerBody)
                                {
                                    CharacterBody.IcicleItemBehavior ib = a.attackerBody.GetComponent<CharacterBody.IcicleItemBehavior>();
                                    if (ib && ib.icicleAura)
                                    {
                                        ib.icicleAura.OnOwnerKillOther();
                                    }
                                }
                                break;
                            default: break;
                        }
                    }
                    pendingAssists.Remove(a);
                }
            }
        }

        public void Awake()
        {
            pendingAssists = new List<Assist>();
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
        }

        private class Assist
        {
            public float timer;
            public CharacterBody attackerBody;
            public CharacterBody victimBody;
            public AssistType assistType;

            public Assist(CharacterBody attackerBody, CharacterBody victimBody, AssistType assistType, float timer)
            {
                this.attackerBody = attackerBody;
                this.victimBody = victimBody;
                this.assistType = assistType;
                this.timer = timer;
            }
        }

        public enum AssistType
        {
            ResetCooldowns, //Convert later
            BanditSkull, //Convert later
            LaserTurbine,
            Berzerk,
            HeadHunter,
            Brainstalks,
            FrostRelic
        }
    }
}
