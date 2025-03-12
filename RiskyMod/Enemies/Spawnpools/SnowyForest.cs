using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class SnowyForest
    {
        public static bool enabled = true;
        public SnowyForest()
        {
            if (!enabled) return;

            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/snowyforest/dccsSnowyForestMonstersDLC1.asset").WaitForCompletion();

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.BlindVerminSnowy, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.Beetle, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.Wisp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.BlindVerminSnowy, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.Bison, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
