using RoR2;
using UnityEngine.Networking;
using UnityEngine;

namespace RiskyMod.Survivors.Mage.Components.Primaries
{
    public class FireBoltRepeatComponent : MonoBehaviour
    {
        public static GameObject damageEffectPrefab;

        public float damage;
        public bool isCrit;
        public GameObject attacker;
        public GameObject victim;
        public HealthComponent victimHealthComponent;
        public DamageTypeCombo damageType;
        public float procCoefficient;
        public TeamIndex teamIndex;
        public float force;

        public float delayBetweenHits = 0.2f;
        public int totalHits = 4;

        private float delayTimer = 0f;

        public void Start()
        {
            teamIndex = TeamIndex.None;
            if (attacker)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    delayBetweenHits = 0.2f / attackerBody.attackSpeed;
                    totalHits = Mathf.Max(totalHits, Mathf.FloorToInt(totalHits * attackerBody.attackSpeed));

                    if (attackerBody.teamComponent)
                    {
                        teamIndex = attackerBody.teamComponent.teamIndex;
                    }
                }
            }

            //Initial hit counts as 1
            totalHits--;
            delayTimer = delayBetweenHits;
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            if (totalHits <= 0)
            {
                Destroy(this);
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

            Vector3 pos = transform.position;
            if (victimHealthComponent && victimHealthComponent.body && victimHealthComponent.body.mainHurtBox)
            {
                pos = victimHealthComponent.body.mainHurtBox.transform.position;
            }

            BlastAttack ba = new BlastAttack()
            {
                baseDamage = damage,
                radius = 2.5f,
                position = pos,
                crit = isCrit,
                procChainMask = default,
                procCoefficient = procCoefficient,
                attacker = attacker,
                inflictor = attacker,
                baseForce = force,
                damageType = damageType,
                falloffModel = BlastAttack.FalloffModel.None,
                teamIndex = teamIndex
            };
            ba.Fire();

            /*DamageInfo damageInfo = new DamageInfo
            {
                damage = damage,
                damageType = damageType,
                procCoefficient = 1f,
                procChainMask = default,
                crit = isCrit,
                attacker = attacker,
                inflictor = attacker,
                force = force,
                position = transform.position
            };

            victimHealthComponent.TakeDamage(damageInfo);
            GlobalEventManager.instance.OnHitEnemy(damageInfo, victim);
            GlobalEventManager.instance.OnHitAll(damageInfo, victim);*/

            EffectManager.SpawnEffect(damageEffectPrefab, new EffectData
            {
                origin = pos,
                scale = 2.5f
            }, true);
        }
    }
}
