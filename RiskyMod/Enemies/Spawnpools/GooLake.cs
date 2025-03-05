using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace RiskyMod.Enemies.Spawnpools
{
    public class GooLake
    {
        public static bool enabled = true;
        public GooLake()
        {
            if (!enabled) return;
            var basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/goolake/dccsGooLakeMonsters.asset").WaitForCompletion();
            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/goolake/dccsGooLakeMonstersDLC1.asset").WaitForCompletion();

            var basePoolIT = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itgoolake/dccsITGooLakeMonsters.asset").WaitForCompletion();
            var dlc1PoolIT = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itgoolake/dccsITGooLakeMonstersDLC1.asset").WaitForCompletion();

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.MagmaWormLoop, SneedUtils.SneedUtils.MonsterCategories.Champions);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.ReminderLoop, SneedUtils.SneedUtils.MonsterCategories.Champions);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePoolIT, DirectorCards.MagmaWormLoop, SneedUtils.SneedUtils.MonsterCategories.Champions);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePoolIT, DirectorCards.ReminderLoop, SneedUtils.SneedUtils.MonsterCategories.Champions);

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.ClayApothecary, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.ClayApothecary, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
