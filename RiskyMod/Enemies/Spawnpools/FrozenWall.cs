using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class FrozenWall
    {
        public FrozenWall()
        {
            DirectorCardCategorySelection basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/frozenwall/dccsFrozenWallMonsters.asset").WaitForCompletion();
            if (EnemiesCore.spawnpoolDLCReplacementFix)
            {
                SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(basePool, SpawnCards.Wisp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            }
        }
    }
}
