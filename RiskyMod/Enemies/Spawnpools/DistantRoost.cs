using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class DistantRoost
    {
        public static bool enabled = true;
        public DistantRoost()
        {
            if (!enabled) return;

            var basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/blackbeach/dccsBlackBeachMonsters.asset").WaitForCompletion();

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Wisp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            //todo: actually make a separate dlc pool for these
            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/blackbeach/dccsBlackBeachMonsters.asset").WaitForCompletion();
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.AlphaConstructLoop, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.XiConstructLoop, SneedUtils.SneedUtils.MonsterCategories.Champions);
        }
    }
}
