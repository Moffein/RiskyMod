using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class StadiaJungle
    {
        public static bool enabled = true;
        public StadiaJungle()
        {
            if (!enabled) return;

            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/rootjungle/dccsRootJungleMonsters.asset").WaitForCompletion());
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/rootjungle/dccsRootJungleMonstersDLC1.asset").WaitForCompletion());
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs)
        {
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Lemurian, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Mushrum, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Dunestrider, SneedUtils.SneedUtils.MonsterCategories.Champions);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Vulture, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Mushrum, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Grovetender, SneedUtils.SneedUtils.MonsterCategories.Champions);
        }
    }
}
