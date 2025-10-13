using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2.Orbs;
using R2API;
using RiskyMod.Survivors.Mage.SkillTweaks;

namespace RiskyMod.Survivors.Mage.Components.Primaries
{
    public class LightningBoltRepeatComponent : MonoBehaviour
    {
        public NetworkSoundEventDef attackSound;
        public GameObject activationEffectPrefab;
        public GameObject whiffEffectPrefab;
        public float range;
        public float delayBetweenHits = 0.2f;
        public int totalHits = 4;

        private float delayTimer = 0f;

        //Copy from projectile
        public float damage;
        public bool isCrit;
        public GameObject attacker;
        public TeamIndex teamIndex;
        public DamageTypeCombo damageType;

        private void Start()
        {
            if (attacker)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    delayBetweenHits = 0.2f / attackerBody.attackSpeed;
                    totalHits = Mathf.Max(totalHits, Mathf.FloorToInt(totalHits * attackerBody.attackSpeed));
                }
            }

            if (NetworkServer.active)
            {
                EffectManager.SpawnEffect(activationEffectPrefab, new EffectData
                {
                    origin = base.transform.position,
                    scale = range
                }, true);
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            if (totalHits <= 0)
            {
                Destroy(base.gameObject);
                return;
            }

            if (delayTimer <= 0f)
            {
                delayTimer += delayBetweenHits;
                TriggerAttackServer();
            }
            delayTimer -= Time.fixedDeltaTime;
        }

        private void TriggerAttackServer()
        {
            if (!NetworkServer.active) return;
            totalHits--;
            List<HealthComponent> enemies = SneedUtils.SneedUtils.FindEnemiesInSphere(range, base.transform.position, teamIndex);
            EffectManager.SimpleSoundEffect(attackSound.index, base.transform.position, true);
            bool fired = false;
            foreach (HealthComponent hc in enemies)
            {
                if (hc.body && hc.body.mainHurtBox && hc.body.mainHurtBox.isActiveAndEnabled)
                {
                    LightningOrb lightning = new LightningOrb
                    {
                        attacker = attacker,
                        inflictor = base.gameObject,
                        damageValue = damage,
                        procCoefficient = 1f,
                        teamIndex = teamIndex,
                        isCrit = isCrit,
                        procChainMask = default,
                        lightningType = LightningOrb.LightningType.Ukulele,
                        damageColorIndex = DamageColorIndex.Default,
                        bouncesRemaining = 0,
                        targetsToFindPerBounce = 1,
                        range = range,
                        origin = base.transform.position,
                        damageType = damageType,
                        speed = 120f,
                        target = hc.body.mainHurtBox
                    };
                    lightning.damageType.AddModdedDamageType(PrimaryRework.QuadrupleHitstunPower);
                    OrbManager.instance.AddOrb(lightning);
                    fired = true;
                }
            }

            if (!fired)
            {
                EffectManager.SimpleEffect(whiffEffectPrefab, base.transform.position, base.transform.rotation, true);
            }
        }
    }
}
