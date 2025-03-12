using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class DampCaveSimple
    {
        public static bool enabled = true;

        public DampCaveSimple()
        {
            if (!enabled) return;
            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/dampcave/dccsDampCaveMonstersDLC1.asset").WaitForCompletion();
            var dlc1PoolIT = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itdampcave/dccsITDampCaveMonsters.asset").WaitForCompletion();

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.Gup, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1PoolIT, SpawnCards.Gup, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
