using R2API;
using RiskyMod.Enemies;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace RiskyMod.Content.Enemies
{
    public class LunarGolemSkyMeadow
    {
        public static GameObject bodyObject;
        public static GameObject masterObject;
        public static CharacterSpawnCard characterSpawnCard;

        public LunarGolemSkyMeadow()
        {
            if (bodyObject) return;
            SetupBody();
            SetupSpawnCard();
        }

        private void SetupBody()
        {
            bodyObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/lunargolembody").InstantiateClone("RiskyMod_LunarGolemSkyMeadow", true);
            DeathRewards dr = bodyObject.GetComponent<DeathRewards>();
            dr.logUnlockableDef = null;
            Content.bodyPrefabs.Add(bodyObject);
            ModifyStats(bodyObject);

            masterObject = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/lunargolemmaster").InstantiateClone("RiskyMod_LunarGolemSkyMeadowMaster", true);
            masterObject.GetComponent<CharacterMaster>().bodyPrefab = bodyObject;
        }

        private void SetupSpawnCard()
        {
            characterSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            characterSpawnCard.name = "cscRiskyModLunarGolemSkyMeadow";
            characterSpawnCard.prefab = masterObject;
            characterSpawnCard.sendOverNetwork = true;
            characterSpawnCard.hullSize = HullClassification.Golem;
            characterSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            characterSpawnCard.requiredFlags = NodeFlags.None;
            characterSpawnCard.forbiddenFlags = NodeFlags.NoCharacterSpawn;
            characterSpawnCard.directorCreditCost = 115;    //115 is Elder Lemurian
            characterSpawnCard.occupyPosition = false;
            characterSpawnCard.loadout = new SerializableLoadout();
            characterSpawnCard.noElites = false;
            characterSpawnCard.forbiddenAsBoss = true;

            DirectorCards.LunarGolemSkyMeadow = DirectorCards.BuildDirectorCard(characterSpawnCard, DirectorAPI.MonsterCategory.Minibosses);
            DirectorCards.LunarGolemSkyMeadowBasic = DirectorCards.BuildDirectorCard(characterSpawnCard, DirectorAPI.MonsterCategory.BasicMonsters);
        }

        private void ModifyStats(GameObject go)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();

            cb.baseMaxHealth = 1000f;    //Vanilla is 1615
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;

            cb.baseDamage = 35f;
            cb.levelDamage = cb.baseDamage * 0.2f;

            cb.baseRegen = 0f;
            cb.levelRegen = cb.baseRegen * 0.2f;
        }
    }
}
