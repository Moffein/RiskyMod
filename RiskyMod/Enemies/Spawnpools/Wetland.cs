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

            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/foggyswamp/dccsFoggySwampMonsters.asset").WaitForCompletion());
            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/foggyswamp/dccsFoggySwampMonstersDLC.asset").WaitForCompletion());
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs)
        {
            bool removedGup = SneedUtils.SneedUtils.RemoveMonsterSpawnCardFromCategory(dccs, Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/Gup/cscGupBody.asset").WaitForCompletion(), SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            if (removedGup)
            {
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.GupLoop, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
                SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Geep, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            }
        }
    }
}
