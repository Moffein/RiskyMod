using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Bosses
{
    public class AWU
    {
        public static bool enabled = true;
        public AWU()
        {
            if (!enabled) return;
            ModifyStats();
        }

        private void ModifyStats()
        {
            GameObject enemyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RoboBallBoss/SuperRoboBallBossBody.prefab").WaitForCompletion();
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();

            cb.baseDamage = 25f;    //orig is 15
            cb.levelDamage = cb.baseDamage * 0.2f;
        }
    }
}
