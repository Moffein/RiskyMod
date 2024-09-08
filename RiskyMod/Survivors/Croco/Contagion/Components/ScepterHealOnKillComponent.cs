using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Croco.Contagion.Components
{
    public class ScepterHealOnKillComponent : MonoBehaviour
    {
        public HealthComponent ownerHealth;
        public HealthComponent victimHealth;

        //dont bother tracking debuffs, just use a timer
        public float timer = 10f;

        public void FixedUpdate()
        {
            if (!NetworkServer.active) return;
            timer -= Time.fixedDeltaTime;
            if (timer <= 0f)
            {
                Destroy(this);
                return;
            }
        }

        public void OnDestroy()
        {
            if (!NetworkServer.active || !ownerHealth || !victimHealth) return;
            if (victimHealth.alive) return;

            ownerHealth.HealFraction(0.05f, default);
            EffectData effectData = new EffectData
            {
                origin = ownerHealth.transform.position
            };
            effectData.SetNetworkedObjectReference(ownerHealth.gameObject);
            EffectManager.SpawnEffect(SharedDamageTypes.medkitEffect, effectData, true);
        }
    }
}
