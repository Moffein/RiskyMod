using RoR2;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class Lakes
    {
        public static bool enabled = true;

        public Lakes()
        {
            if (!enabled) return;
            var dlc1Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lakes/dccsLakesMonstersDLC1.asset").WaitForCompletion();
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dlc1Pool, DirectorCards.GupLoop, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
        }
    }
}
