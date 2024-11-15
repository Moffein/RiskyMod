using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Spawnpools
{
    public class VillageNight
    {
        public static bool enabled = true;
        public VillageNight()
        {

            if (!enabled) return;


            List<SpawnCard> cardsToModify = new List<SpawnCard>()
            {
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/LemurianBruiser/cscLemurianBruiser.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Parent/cscParent.asset").WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Grandparent/cscGrandparent.asset").WaitForCompletion()
            };

            ModifyCards(Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC2/villagenight/dccsVillageNightMonsters.asset").WaitForCompletion(), cardsToModify);
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

            dccs.AddCard(SneedUtils.SneedUtils.FindCategoryIndexByName(dccs, "Minibosses"), DirectorCards.Golem);
        }
    }
}
