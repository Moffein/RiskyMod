using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class SkyMeadow
    {
        public static bool enabled = true;
        public SkyMeadow()
        {
            if (!enabled) return;

            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/skymeadow/dccsSkyMeadowMonsters.asset").WaitForCompletion(), false, false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/skymeadow/dccsSkyMeadowMonstersDLC1.asset").WaitForCompletion(), true, false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itskymeadow/dccsITSkyMeadowMonsters.asset").WaitForCompletion(), true, true);
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs, bool isDLC, bool isSimulacrum)
        {

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.AlphaConstruct, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.XiConstruct, SneedUtils.SneedUtils.MonsterCategories.Champions);

            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Mushrum, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Wisp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Bronzong, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Jellyfish, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Imp, SneedUtils.SneedUtils.MonsterCategories.BasicMonsters);

            //Dont know if DLC removes the worms or not. Reset them just to be safe.
            if (!isSimulacrum)
            {
                SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.MagmaWorm, SneedUtils.SneedUtils.MonsterCategories.Champions);
                SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.Reminder, SneedUtils.SneedUtils.MonsterCategories.Champions);

                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.MagmaWorm, SneedUtils.SneedUtils.MonsterCategories.Champions);
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Reminder, SneedUtils.SneedUtils.MonsterCategories.Champions);
            }
        }
    }
}
