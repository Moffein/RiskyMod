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

            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/golemplains/dccsGolemplainsMonsters.asset").WaitForCompletion(), false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/golemplains/dccsGolemplainsMonstersDLC1.asset").WaitForCompletion(), false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/dccsGolemplainsMonstersDLC2.asset").WaitForCompletion(), false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/dccsGolemplainsMonstersDLC2Only.asset").WaitForCompletion(), false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itgolemplains/dccsITGolemplainsMonsters.asset").WaitForCompletion(), true);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itgolemplains/dccsITGolemplainsMonstersDLC2.asset").WaitForCompletion(), true);
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs, bool stage1Bisons)
        {
            bool removedJelly = SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            bool removedAlpha = SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.AlphaConstruct, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            bool removedXi = SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.XiConstruct, SneedUtils.SneedUtils.MonsterCategories.Champions);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, stage1Bisons ? DirectorCards.Bison : DirectorCards.BisonLoop, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
