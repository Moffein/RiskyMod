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

            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/blackbeach/dccsBlackBeachMonsters.asset").WaitForCompletion(), false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/blackbeach/dccsBlackBeachMonstersDLC.asset").WaitForCompletion(), true);
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs, bool isDLC)
        {
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Wisp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            if (isDLC) SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.AlphaConstructLoop, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
        }
    }
}
