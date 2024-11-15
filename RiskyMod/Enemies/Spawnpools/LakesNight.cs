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

            ModifyCards(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lakesnight/dccsLakesnightMonsters.asset").WaitForCompletion(), cardsToModify);
            ModifyCards(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/lakesnight/dccsLakesnightMonstersStormOnlyDLC2.asset").WaitForCompletion(), cardsToModify);
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

            dccs.AddCard(SneedUtils.SneedUtils.FindCategoryIndexByName(dccs, "Minibosses"), DirectorCards.GolemNature);
        }
    }
}
