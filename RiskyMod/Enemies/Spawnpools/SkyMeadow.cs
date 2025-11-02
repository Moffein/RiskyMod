using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class SkyMeadow
    {
        public static bool enabled = true;
        public SkyMeadow()
        {
            if (!enabled) return;

            var basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/skymeadow/dccsSkyMeadowMonsters.asset").WaitForCompletion();
            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/skymeadow/dccsSkyMeadowMonstersDLC1.asset").WaitForCompletion();
            var dlc2Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/dccsSkyMeadowMonstersDLC2.asset").WaitForCompletion();

            var basePoolIT = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itskymeadow/dccsITSkyMeadowMonsters.asset").WaitForCompletion();
            var dlc2PoolIT = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itskymeadow/dccsITSkyMeadowMonstersDLC2.asset").WaitForCompletion();

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Mushrum, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Bronzong, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.Mushrum, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.Bronzong, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.Imp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePoolIT, DirectorCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePoolIT, DirectorCards.Imp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            if (DirectorCards.LunarGolemSkyMeadow != null)
            {
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.LunarGolemSkyMeadow, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePoolIT, DirectorCards.LunarGolemSkyMeadow, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            }

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.AlphaConstruct, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.XiConstruct, SneedUtils.SneedUtils.MonsterCategories.Champions);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.Gup, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.AlphaConstruct, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.XiConstruct, SneedUtils.SneedUtils.MonsterCategories.Champions);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.Gup, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
