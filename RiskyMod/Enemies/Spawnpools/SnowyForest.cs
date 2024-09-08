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
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/snowyforest/dccsSnowyForestMonstersDLC1.asset").WaitForCompletion());
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/dccsSnowyForestMonstersDLC2.asset").WaitForCompletion());
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs)
        {
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.BlindVerminSnowy, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Beetle, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Wisp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.BlindVerminSnowy, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Bison, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
