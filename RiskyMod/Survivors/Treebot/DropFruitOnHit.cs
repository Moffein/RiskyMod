using RoR2;
using UnityEngine;
using RiskyMod.SharedHooks;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Treebot
{
    public class DropFruitOnHit
    {
        public static bool enabled = true;
        public static GameObject fruitEffectPrefab;
        public static GameObject fruitPrefab;

        public DropFruitOnHit()
        {
            if (!enabled) return;

            fruitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotFruitDeathEffect.prefab").WaitForCompletion();
            fruitPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotFruitPack.prefab").WaitForCompletion();
            OnHitEnemy.OnHitAttackerActions += FruitOnHit;
        }

        private static void FruitOnHit(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (victimBody.HasBuff(RoR2Content.Buffs.Fruiting.buffIndex))
            {
                if (Util.CheckRoll(15f * damageInfo.procCoefficient, attackerBody.master))
                {
                    EffectManager.SpawnEffect(fruitEffectPrefab, new EffectData
                    {
                        origin = victimBody.corePosition,
                        rotation = UnityEngine.Random.rotation
                    }, true);

                    GameObject instantiated = UnityEngine.Object.Instantiate<GameObject>(fruitPrefab, victimBody.corePosition
                        + UnityEngine.Random.insideUnitSphere * victimBody.radius * 0.5f, UnityEngine.Random.rotation);
                    instantiated.GetComponent<TeamFilter>().teamIndex = attackerBody.teamComponent.teamIndex;
                    instantiated.GetComponentInChildren<HealthPickup>();
                    instantiated.transform.localScale = new Vector3(1f, 1f, 1f);
                    NetworkServer.Spawn(instantiated);
                }
            }
        }
    }
}
