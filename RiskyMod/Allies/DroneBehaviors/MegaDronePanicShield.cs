using RoR2.Orbs;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DroneBehaviors
{
    public class MegaDronePanicShield : MonoBehaviour
    {
        public static float baseCooldown = 130f;
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

            /*new BlastAttack
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
            }.Fire();*/
            List<HealthComponent> bouncedObjects = new List<HealthComponent>();
            int targets = 20;
            float range = 30f;

            //Need to individually find all targets for the first bounce.
            for (int i = 0; i < targets; i++)
            {
                LightningOrb shockLightning = new LightningOrb
                {
                    bouncedObjects = bouncedObjects,
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    damageValue = 0f,
                    procCoefficient = 1f,
                    teamIndex = body.teamComponent ? body.teamComponent.teamIndex : TeamIndex.None,
                    isCrit = false,
                    procChainMask = default,
                    lightningType = LightningOrb.LightningType.Tesla,
                    damageColorIndex = DamageColorIndex.Nearby,
                    bouncesRemaining = 1,
                    targetsToFindPerBounce = 2,
                    range = range,
                    origin = base.transform.position,
                    damageType = DamageType.Shock5s,
                    speed = 120f
                };

                HurtBox hurtBox = shockLightning.PickNextTarget(base.transform.position);

                //Fire orb if HurtBox is found.
                if (hurtBox)
                {
                    shockLightning.target = hurtBox;
                    shockLightning.range *= 0.5f; //bounces have reduced range compared to the initial bounce
                    OrbManager.instance.AddOrb(shockLightning);
                    shockLightning.bouncedObjects.Add(hurtBox.healthComponent);
                }
            }
        }
    }
}
