using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class LakesNight
    {
        public static bool enabled = true;
        public LakesNight()
        {

            if (!enabled) return;


            List<SpawnCard> cardsToModify = new List<SpawnCard>()
            {
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/LemurianBruiser/cscLemurianBruiser.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Parent/cscParent.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Nullifier/cscNullifier.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/Gup/cscGupBody.asset").WaitForCompletion(),
            };

            var basePool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lakesnight/dccsLakesnightMonsters.asset").WaitForCompletion();
            var dlc2Pool = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lakesnight/dccsLakesnightMonsters_DLC1.asset").WaitForCompletion();

            ModifyCards(basePool, cardsToModify);
            ModifyCards(dlc2Pool, cardsToModify);
        }

        private void ModifyCards(DirectorCardCategorySelection dccs, List<SpawnCard> cardList)
        {
            foreach (var category in dccs.categories)
            {
                foreach (var card in category.cards)
                {
                    if (card.minimumStageCompletions < 3 && cardList.Contains(card.spawnCard))
                    {
                        card.minimumStageCompletions = 3;
                    }
                }
            }

            //I am going to become the joker
            var index = SneedUtils.SneedUtils.FindCategoryIndexByName(dccs, SneedUtils.SneedUtils.MonsterCategories.Minibosses);
            if (index == -1) index = SneedUtils.SneedUtils.FindCategoryIndexByName(dccs, "Mini Bosses");

            if (index != -1)
            {
                dccs.AddCard(index, DirectorCards.GolemNature);
            }
        }
    }
}
