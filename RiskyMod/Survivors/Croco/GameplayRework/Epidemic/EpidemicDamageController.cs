using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Croco
{
    public class EpidemicDamageController : MonoBehaviour
    {
        public static int baseTickCount = 5;    //Initial hit is 1 tick
        public static int baseTickCountScepter = 9;
        public static float timeBetweenTicks = 0.5f;
        public static float baseLingerDuration = 1f;
        public static float damageCoefficient = 1f;

        public static GameObject impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/crocodiseaseimpacteffect");

        public CharacterBody owner;
        public CharacterBody victim;
        private int ticksRemaining;
        private float stopwatch;
        private float lingerStopwatch;
        private bool scepter = false;
        private bool spreadOnDeath = false;
        private bool victimKilled = false;

        public bool crit = false;
        public float damage = 0f;

        private void Awake()
        {
            if (!NetworkServer.active)
            {
                Destroy(this);
                return;
            }
            stopwatch = 0f;
            lingerStopwatch = 0f;
            ticksRemaining = scepter ? baseTickCountScepter : baseTickCount;
        }

        private void FixedUpdate()
        {
            if (victim && !victim.HasBuff(ModifySpecial.EpidemicDebuff.buffIndex))
            {
                victim.AddBuff(ModifySpecial.EpidemicDebuff.buffIndex);
            }

            if (ticksRemaining > 0)
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= timeBetweenTicks)
                {
                    stopwatch -= timeBetweenTicks;
                    ticksRemaining--;
                    if (owner && victim && victim.healthComponent)
                    {
                        DamageInfo diseaseDamage = new DamageInfo
                        {
                            attacker = owner.gameObject,
                            inflictor = owner.gameObject,
                            damage = this.damage,
                            damageColorIndex = DamageColorIndex.Default,
                            damageType = DamageType.Generic,
                            crit = this.crit,
                            dotIndex = DotController.DotIndex.None,
                            force = 300f * Vector3.down,
                            position = victim.corePosition,
                            procChainMask = default(ProcChainMask),
                            procCoefficient = 0.5f
                        };
                        victim.healthComponent.TakeDamage(diseaseDamage);
                        GlobalEventManager.instance.OnHitEnemy(diseaseDamage, victim.gameObject);

                        EffectManager.SimpleImpactEffect(impactEffect, victim.corePosition, Vector3.up, true);
                    }
                }
            }
            else
            {
                lingerStopwatch += Time.fixedDeltaTime;
            }

            victimKilled = !(victim && victim.healthComponent && victim.healthComponent.alive);
            //Let the debuff last a bit longer than the damage duration.
            if (lingerStopwatch >= baseLingerDuration || victimKilled)
            {
                Destroy(this);
                return;
            }
        }

        private void OnDestroy()
        {
            if (victim && victim.HasBuff(ModifySpecial.EpidemicDebuff.buffIndex))
            {
                victim.RemoveBuff(ModifySpecial.EpidemicDebuff.buffIndex);
            }
            if (scepter && owner && !(victim && victim.healthComponent && victim.healthComponent.alive))
            {
                owner.AddTimedBuff(RegenRework.CrocoRegen2, RegenRework.regenDuration);
            }

            if (spreadOnDeath && victimKilled && victim && owner)
            {
                CrocoAltPassiveTracker.TriggerPoisonSpreadModdedDamage(owner, victim, scepter ? ModifySpecial.EpidemicScepter : ModifySpecial.Epidemic);
            }
        }

        public void SetScepter()
        {
            ticksRemaining = baseTickCountScepter;
            scepter = true;
        }

        public void SetPassive(DamageType dt)
        {
            if (dt == DamageType.PoisonOnHit)
            {
                ticksRemaining = Mathf.FloorToInt(ticksRemaining * 1.4f);
            }
            else if (dt == DamageType.BlightOnHit)
            {
                spreadOnDeath = true;
            }
        }
    }
}
