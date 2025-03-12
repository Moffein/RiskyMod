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

            var basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/rootjungle/dccsRootJungleMonsters.asset").WaitForCompletion();

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Lemurian, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Mushrum, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Dunestrider, SneedUtils.SneedUtils.MonsterCategories.Champions);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.Vulture, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.Mushrum, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(basePool, DirectorCards.Grovetender, SneedUtils.SneedUtils.MonsterCategories.Champions);

            if (EnemiesCore.spawnpoolDLCReplacementFix)
            {
                SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.LemurianBruiser, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            }
        }
    }
}
