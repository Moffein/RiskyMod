using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Croco
{
    public class EpidemicDamageController : MonoBehaviour
    {

        public static float baseLingerDuration = 1f;

        public static GameObject impactEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/crocodiseaseimpacteffect");

        public CharacterBody owner;
        public CharacterBody victim;
        private int ticksRemaining;
        private float stopwatch;
        private float lingerStopwatch;
        private bool scepter = false;
        //private bool spreadOnDeath = false;
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
            ticksRemaining = CrocoCore.Cfg.Skills.Epidemic.baseTickCount * (scepter ? 2 : 1);
        }

        public void Setup(CharacterBody attackerBody, CharacterBody victimBody, DamageInfo damageInfo, bool isScepter = false)
        {
            owner = attackerBody;
            victim = victimBody;
            damage = damageInfo.damage;
            crit = damageInfo.crit;
            victim.AddBuff(ModifySpecial.EpidemicDebuff.buffIndex);
            if (isScepter)
            {
                SetScepter();
            }
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
                if (stopwatch >= CrocoCore.Cfg.Skills.Epidemic.timeBetweenTicks)
                {
                    stopwatch -= CrocoCore.Cfg.Skills.Epidemic.timeBetweenTicks;
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
                            procCoefficient = CrocoCore.Cfg.Skills.Epidemic.procCoefficient
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
                owner.healthComponent.HealFraction(0.05f, default(ProcChainMask));
                EffectData effectData = new EffectData
                {
                    origin = owner.corePosition
                };
                effectData.SetNetworkedObjectReference(owner.gameObject);
                EffectManager.SpawnEffect(SharedDamageTypes.medkitEffect, effectData, true);
            }

            /*if (spreadOnDeath && victimKilled && victim && owner)
            {
                CrocoAltPassiveTracker.TriggerPoisonSpreadModdedDamage(owner, victim, scepter ? ModifySpecial.EpidemicScepter : ModifySpecial.Epidemic);
            }*/
        }

        public void SetScepter()
        {
            ticksRemaining = CrocoCore.Cfg.Skills.Epidemic.baseTickCount * 2;
            scepter = true;
        }
    }
}
