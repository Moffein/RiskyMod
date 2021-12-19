using RoR2;
using UnityEngine;
using R2API;
using RoR2.Projectile;
using System.Collections.Generic;
using RoR2.Orbs;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyPassives
    {
        public static CrocoAltPassiveTracker PoisonTrackerInstance;
        public ModifyPassives()
        {
            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                ModifyPassives.PoisonTrackerInstance = self.gameObject.AddComponent<CrocoAltPassiveTracker>();
            };

            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool isPoison = (damageInfo.damageType & DamageType.PoisonOnHit) == DamageType.PoisonOnHit;
                bool isBlight = (damageInfo.damageType & DamageType.BlightOnHit) == DamageType.BlightOnHit;
                if (isPoison || isBlight)
                {
                    if (damageInfo.attacker)
                    {
                        CrocoDamageTypeController cd = damageInfo.attacker.GetComponent<CrocoDamageTypeController>();
                        if (cd)
                        {
                            switch (cd.GetDamageType())
                            {
                                case DamageType.PoisonOnHit:    //Passive: Extended poison duration
                                    if (isBlight)
                                    {
                                        damageInfo.damageType &= ~DamageType.BlightOnHit;
                                        damageInfo.AddModdedDamageType(SharedDamageTypes.Blight7s);
                                    }
                                    break;
                                case DamageType.BlightOnHit:    //Passive: Poison spread on application
                                    if (isPoison)
                                    {
                                        damageInfo.damageType &= ~DamageType.PoisonOnHit;
                                        damageInfo.AddModdedDamageType(SharedDamageTypes.Poison7s);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                orig(self, damageInfo);
            };

            SharedHooks.OnHitEnemy.OnHitAttackerActions += TrackPoison;
        }

        private static void TrackPoison(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            CrocoDamageTypeController cd = attackerBody.GetComponent<CrocoDamageTypeController>();
            if (cd && cd.GetDamageType() == DamageType.BlightOnHit)
            {
                bool isBlight = (damageInfo.damageType & DamageType.BlightOnHit) == DamageType.BlightOnHit;
                bool isPoison = damageInfo.HasModdedDamageType(SharedDamageTypes.Poison7s);
                if (isBlight)
                {
                    ModifyPassives.PoisonTrackerInstance.Add(attackerBody, victimBody, DamageType.BlightOnHit, 5f);
                }
                if (isPoison)
                {
                    ModifyPassives.PoisonTrackerInstance.Add(attackerBody, victimBody, DamageType.PoisonOnHit, 7f);
                }
            }
        }
    }

    public class CrocoAltPassiveTracker : MonoBehaviour
    {
        public void Add(CharacterBody attackerBody, CharacterBody victimBody, DamageType damageType, float duration)
        {
            CrocoPoison existing = poisonList.Find(x => x.victimBody == victimBody && x.damageType == damageType);
            if (existing != null)
            {
                existing.attackerBody = attackerBody;
                existing.duration = duration;
            }
            else
            {
                poisonList.Add(new CrocoPoison(attackerBody, victimBody, damageType, duration));
            }
        }

        private void Awake()
        {
            poisonList = new List<CrocoPoison>();
        }

        private void FixedUpdate()
        {
            if (poisonList.Count > 0)
            {
                List<CrocoPoison> killList = new List<CrocoPoison>();
                List<CrocoPoison> toRemove = new List<CrocoPoison>();

                foreach (CrocoPoison c in poisonList)
                {
                    c.duration -= Time.fixedDeltaTime;
                    if (c.duration >= 0f && c.victimBody && c.victimBody.healthComponent)
                    {
                        if (!c.victimBody.healthComponent.alive)
                        {
                            killList.Add(c);
                        }
                    }
                    else
                    {
                        toRemove.Add(c);
                    }
                }

                foreach (CrocoPoison c in toRemove)
                {
                    poisonList.Remove(c);
                }

                foreach (CrocoPoison c in killList)
                {
                    poisonList.Remove(c);
                    TriggerPoisonSpread(c.attackerBody, c.victimBody, c.damageType);
                }
            }
        }

        public static void TriggerPoisonSpread(CharacterBody attackerBody, CharacterBody victimBody, DamageType damageType)
        {
            LightningOrb lightningOrb = new LightningOrb();
            lightningOrb.arrivalTime = OrbManager.instance.time + 0.5f;
            lightningOrb.bouncedObjects = new List<HealthComponent>();
            lightningOrb.targetsToFindPerBounce = 1;
            lightningOrb.canBounceOnSameTarget = false;
            lightningOrb.attacker = attackerBody.gameObject;
            lightningOrb.inflictor = attackerBody.gameObject;
            lightningOrb.teamIndex = attackerBody.teamComponent.teamIndex;
            lightningOrb.damageValue = attackerBody.damage * 0.5f;
            lightningOrb.isCrit = attackerBody.RollCrit();
            lightningOrb.origin = victimBody.corePosition;
            lightningOrb.bouncesRemaining = 3;
            lightningOrb.lightningType = LightningOrb.LightningType.CrocoDisease;
            lightningOrb.target = lightningOrb.PickNextTarget(victimBody.corePosition);
            lightningOrb.damageColorIndex = DamageColorIndex.Poison;
            lightningOrb.damageType = damageType;
            lightningOrb.procCoefficient = 1f;
            lightningOrb.range = 15f;

            lightningOrb.bouncedObjects.Add(victimBody.healthComponent);
            OrbManager.instance.AddOrb(lightningOrb);
        }

        private List<CrocoPoison> poisonList;
        public class CrocoPoison
        {
            public CrocoPoison(CharacterBody attacker, CharacterBody victim, DamageType dt, float dur)
            {
                attackerBody = attacker;
                victimBody = victim;
                damageType = dt;
                duration = dur;
            }
            public CharacterBody attackerBody;
            public CharacterBody victimBody;
            public float duration;
            public DamageType damageType;
        }
    }
}
