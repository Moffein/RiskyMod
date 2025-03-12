using EntityStates.Mage.Weapon;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class Wetland
    {
        public static bool enabled = true;
        
        public Wetland()
        {
            if (!enabled) return;

            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/foggyswamp/dccsFoggySwampMonstersDLC.asset").WaitForCompletion();
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.Gup, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.GupLoop, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.Geep, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
        }
    }
}
