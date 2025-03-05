using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class SirensCall
    {
        public static bool enabled = true;
        public SirensCall()
        {

            if (!enabled) return;

            ApplyChanges(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/shipgraveyard/dccsShipgraveyardMonsters.asset").WaitForCompletion());
        }

        private void ApplyChanges(DirectorCardCategorySelection dccs)
        {
            SneedUtils.SneedUtils.AddMonsterDirectorCardToCategory(dccs, DirectorCards.Reminder, SneedUtils.SneedUtils.MonsterCategories.Champions);
        }
    }
}
