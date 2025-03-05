using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class TitanicPlains
    {
        public static bool enabled = true;
        public TitanicPlains()
        {
            if (!enabled) return;

            var basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/golemplains/dccsGolemplainsMonsters.asset").WaitForCompletion();
            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/golemplains/dccsGolemplainsMonstersDLC1.asset").WaitForCompletion();

            var basePoolIT = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itgolemplains/dccsITGolemplainsMonsters.asset").WaitForCompletion();

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.AlphaConstruct, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.XiConstruct, SneedUtils.SneedUtils.MonsterCategories.Champions);

            //bool removedA = SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.AlphaConstruct, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            //bool removedXi = SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePoolIT, SpawnCards.XiConstruct, SneedUtils.SneedUtils.MonsterCategories.Champions);
            //Debug.Log("REmoved from TP Simulacrum: Alpha " + removedA + ", Xi " + removedXi);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.BisonLoop, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePoolIT, DirectorCards.Bison, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
