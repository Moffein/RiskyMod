using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using RoR2.Projectile;

namespace RiskyMod.Survivors.Mage.Components.Primaries
{
    public class LightningBoltTriggerComponent : MonoBehaviour
    {
        public static GameObject lightningboltRepeatObject;
        public float damage;
        public bool isCrit;
        public GameObject attacker;
        public TeamIndex teamIndex;
        public DamageTypeCombo damageType;

        private void Start()
        {
            ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
            if (pd)
            {
                damage = pd.damage;
                isCrit = pd.crit;
                damageType = pd.damageType;
            }
            else
            {
                damage = 0f;
            }

            ProjectileController pc = base.GetComponent<ProjectileController>();
            attacker = pc.owner;

            TeamFilter tf = base.GetComponent<TeamFilter>();
            if (tf)
            {
                teamIndex = tf.teamIndex;
            }
            else
            {
                teamIndex = TeamIndex.None;
            }

            ProjectileImpactExplosion pie = base.GetComponent<ProjectileImpactExplosion>();
            pie.OnProjectileExplosion += SpawnLightningObject;
        }

        private void SpawnLightningObject(BlastAttack attack, BlastAttack.Result result)
        {
            GameObject repeatObject = GameObject.Instantiate(lightningboltRepeatObject, attack.position, Quaternion.identity);

            LightningBoltRepeatComponent lrc = repeatObject.GetComponent<LightningBoltRepeatComponent>();
            lrc.attacker = attacker;
            lrc.isCrit = isCrit;
            lrc.damage = damage;
            lrc.teamIndex = teamIndex;
            lrc.damageType = damageType;

            NetworkServer.Spawn(repeatObject);
        }
    }
}
