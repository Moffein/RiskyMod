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

            SharedHooks.OnHitEnemy.OnHitAttackerActions += TrackPoison;
        }

        private static void TrackPoison(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            CrocoDamageTypeController cd = attackerBody.GetComponent<CrocoDamageTypeController>();
            if (cd && cd.GetDamageType() == DamageType.BlightOnHit)
            {
                bool isBlight = damageInfo.HasModdedDamageType(SharedDamageTypes.CrocoBlight6s);
                bool isPoison = damageInfo.HasModdedDamageType(SharedDamageTypes.CrocoPoison6s);
                if (isBlight)
                {
                    ModifyPassives.PoisonTrackerInstance.Add(attackerBody, victimBody, SharedDamageTypes.CrocoBlight6s, 6f);
                }
                if (isPoison)
                {
                    ModifyPassives.PoisonTrackerInstance.Add(attackerBody, victimBody, SharedDamageTypes.CrocoPoison6s, 6f);
                }
            }
        }
    }

    public class CrocoAltPassiveTracker : MonoBehaviour
    {
        public static float spreadRange = 30f;
        public void Add(CharacterBody attackerBody, CharacterBody victimBody,DamageAPI.ModdedDamageType damageType, float duration)
        {
            CrocoPoison existing = poisonList.Find(x => x.victimBody == victimBody && x.damageType == damageType);
            if (existing != null)
            {
                existing.attackerBody = attackerBody;
                existing.duration = duration;
                existing.stacks++;
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
                    TriggerPoisonSpread(c.attackerBody, c.victimBody, c.damageType, c.stacks);
                }
            }
        }

        public static void TriggerPoisonSpread(CharacterBody attackerBody, CharacterBody victimBody, DamageAPI.ModdedDamageType damageType, int stacks)
        {
            LightningOrb lightningOrb = new LightningOrb();
            lightningOrb.arrivalTime = OrbManager.instance.time + 0.4f;
            lightningOrb.bouncedObjects = new List<HealthComponent>();
            lightningOrb.targetsToFindPerBounce = 1;
            lightningOrb.canBounceOnSameTarget = false;
            lightningOrb.attacker = attackerBody.gameObject;
            lightningOrb.inflictor = attackerBody.gameObject;
            lightningOrb.teamIndex = attackerBody.teamComponent.teamIndex;
            lightningOrb.damageValue = stacks;
            lightningOrb.damageCoefficientPerBounce = 1f;
            lightningOrb.procCoefficient = 0.1f;
            lightningOrb.isCrit = attackerBody.RollCrit();
            lightningOrb.origin = victimBody.corePosition;
            lightningOrb.bouncesRemaining = 0;
            lightningOrb.lightningType = LightningOrb.LightningType.CrocoDisease;
            lightningOrb.damageColorIndex = DamageColorIndex.Poison;
            lightningOrb.damageType = DamageType.NonLethal;
            lightningOrb.range = CrocoAltPassiveTracker.spreadRange;
            lightningOrb.AddModdedDamageType(damageType);

            if (damageType == SharedDamageTypes.CrocoBlight6s) lightningOrb.AddModdedDamageType(SharedDamageTypes.CrocoBlightStack);
            if (Items.Common.Crowbar.enabled) lightningOrb.AddModdedDamageType(Items.Common.Crowbar.IgnoreCrowbar);

            lightningOrb.bouncedObjects.Add(victimBody.healthComponent);
            lightningOrb.target = lightningOrb.PickNextTarget(victimBody.corePosition);
            OrbManager.instance.AddOrb(lightningOrb);
        }

        private List<CrocoPoison> poisonList;
        public class CrocoPoison
        {
            public CrocoPoison(CharacterBody attacker, CharacterBody victim, DamageAPI.ModdedDamageType dt, float dur)
            {
                attackerBody = attacker;
                victimBody = victim;
                damageType = dt;
                duration = dur;
                stacks = 1;
            }
            public CharacterBody attackerBody;
            public CharacterBody victimBody;
            public float duration;
            public DamageAPI.ModdedDamageType damageType;
            public int stacks;
        }
    }
}
