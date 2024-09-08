using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace RiskyMod.Enemies.Spawnpools
{
    public class GooLake
    {
        public static bool enabled = true;
        public GooLake()
        {
            if (!enabled) return;
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/goolake/dccsGooLakeMonsters.asset").WaitForCompletion(), false, false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/goolake/dccsGooLakeMonstersDLC1.asset").WaitForCompletion(), true, false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/itgoolake/dccsITGooLakeMonsters.asset").WaitForCompletion(), true, true);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/dccsGooLakeMonstersDLC2.asset").WaitForCompletion(), true, false);
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/dccsGooLakeMonstersDLC2Only.asset").WaitForCompletion(), false, false);
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs, bool isDLC, bool isSimulacrum)
        {
            if (!isSimulacrum)
            {

                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.MagmaWormLoop, SneedUtils.SneedUtils.MonsterCategories.Champions);
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.ReminderLoop, SneedUtils.SneedUtils.MonsterCategories.Champions);

                if (isDLC)
                {
                    SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, SpawnCards.ClayApothecary, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
                    SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.ClayApothecary, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
                }
            }
            else
            {
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.MagmaWorm, SneedUtils.SneedUtils.MonsterCategories.Champions);
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Reminder, SneedUtils.SneedUtils.MonsterCategories.Champions);
            }
        }
    }
}
