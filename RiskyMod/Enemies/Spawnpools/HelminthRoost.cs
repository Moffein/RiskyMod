﻿using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class HelminthRoost
    {
        public static bool enabled = true;
        
        public HelminthRoost()
        {
            if (!enabled) return;

            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/helminthroost/dccsHelminthRoostMonsters.asset").WaitForCompletion();
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dlc1Pool, SpawnCards.Gup, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
