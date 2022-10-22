using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DroneBehaviors
{
    public class MegaDronePanicShield : MonoBehaviour
    {
        public static float baseCooldown = 100f;
        public static float buffDuration = 10f;
        public static GameObject shockEffectPrefab;

        private float cooldownStopwatch;
        private HealthComponent healthComponent;
        private CharacterBody body;

        public void Awake()
        {
            body = base.GetComponent<CharacterBody>();
            healthComponent = base.GetComponent<HealthComponent>();
            cooldownStopwatch = 0f;

            if (!body || !healthComponent) Destroy(this);
        }

        public void FixedUpdate()
        { 
            if (NetworkServer.active)
            {
                if (cooldownStopwatch <= 0f)
                {
                    if (healthComponent.combinedHealthFraction <= 0.25f)
                    {
                        TriggerShield();
                        cooldownStopwatch = baseCooldown;
                    }
                }
                else
                {
                    cooldownStopwatch -= Time.fixedDeltaTime;
                }
            }
        }

        public void TriggerShield()
        {
            float blastRadius = 20f;

            body.AddTimedBuff(RoR2Content.Buffs.EngiShield, MegaDronePanicShield.buffDuration);

            EffectManager.SpawnEffect(shockEffectPrefab, new EffectData { origin = base.transform.position, scale = blastRadius }, true);

            new BlastAttack
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                baseDamage = body.damage * 0f,
                baseForce = 0f,
                bonusForce = default,
                canRejectForce = true,
                crit = false,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Shock5s,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = base.gameObject,
                teamIndex = body.teamComponent? body.teamComponent.teamIndex : TeamIndex.None,
                position = base.transform.position,
                procChainMask = default,
                procCoefficient = 1f,
                radius = blastRadius
            }.Fire();
        }
    }
}
